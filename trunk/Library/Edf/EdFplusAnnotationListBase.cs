//
// NeuroLoopGain Library
// Library containing helper classes used to implement the NeuroLoopGain analysis.
//
// Copyright 2012 Marco Roessen
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NeuroLoopGainLibrary.DateTimeTypes;
using NeuroLoopGainLibrary.Mathematics;

namespace NeuroLoopGainLibrary.Edf
{
  public class EdfPlusAnnotationListBase : IList<EdfPlusAnnotation>
  {
    #region Delegates and Events

    #region Delegates

    public delegate int TALGetData(object sender, int blockNr, ref bool readOk);
    public delegate void TALReadDataError(object sender, int blockNr, int annotationSignalNr, ref string annotationToAdd, TalError error);

    #endregion Delegates

    #endregion Delegates and Events

    #region Private Fields

    private bool _allBlocksAvailable;
    private double _blockDuration;
    private EdfPlusAnnotation _current;
    private HPDateTime _fileStartDateTime;
    private readonly IFormatProvider _formatInfo;
    private TalError _lastError;
    private int _updateCount;

    #endregion Private Fields

    #region Private Methods

    private double GetTFirstStart()
    {
      return FirstAnnotationStart.SecDifference(FileStartDateTime);
    }

    private double GetTLastEnd()
    {
      return LastAnnotationEnd.SecDifference(FileStartDateTime);
    }

    private bool GetUpdateEnabled()
    {
      return (_updateCount == 0);
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    #endregion Private Methods

    #region Protected Fields

    protected List<EdfPlusAnnotation> AnnotationsList;
    protected List<EdfPlusAnnotation> BookMarks = new List<EdfPlusAnnotation>();

    #endregion Protected Fields

    #region Protected Properties

    protected bool AllBlocksAvailable { get { return GetAllBlocksAvailable(); } }

    protected List<EdfPlusAnnotationDataBlock> BlocksList
    {
      get;
      set;
    }

    #endregion Protected Properties

    #region Protected Methods

    protected virtual void DoAddBookMark()
    {
      BookMarks.Add(_current);
    }

    protected virtual void DoReleaseBookMark()
    {
      _current = BookMarks[BookMarks.Count - 1];
      BookMarks.Remove(_current);
    }

    protected virtual void DoSetBlocksListCapacity(int blockNr)
    {
      DoSetBlocksListCapacity(blockNr, true);
    }

    protected virtual void DoSetBlocksListCapacity(int blockNr, bool isInternal)
    {
      Debug.Assert(blockNr >= 0, TALConsts.InvalidBlockIndex);
      // Blocks are numbered from 0..N-1 => reserve space for N elements
      if (blockNr >= BlocksList.Capacity)
        if (isInternal)
          BlocksList.Capacity = (int)(Math.Ceiling(blockNr / 1024d) * 1024) + 1;
        else
          BlocksList.Capacity = blockNr + 1;
    }

    protected void DoSortAnnotations()
    {
      lock (AnnotationsList)
        AnnotationsList.Sort(new Comparison<EdfPlusAnnotation>(EdfPlusAnnotation.CompareEdfPlusAnnotation));
    }

    protected bool GetAllBlocksAvailable()
    {
      bool result = _allBlocksAvailable;
      if (!result)
      {
        int i = 0;
        while ((i < BlockCount) && BlockInMemory(i))
          i++;
        result = (i == BlockCount);
        _allBlocksAvailable = result;
      }
      return result;
    }

    protected virtual EdfPlusAnnotationDataBlock GetBlock(int index)
    {
      lock (BlocksList)
      {
        Debug.Assert(index >= 0, TALConsts.InvalidBlockIndex);
        _lastError = TalError.None;
        if (index < BlocksList.Count)
          return BlocksList[index];
        return null;
      }
    }

    protected virtual int GetBlockCount()
    {
      _lastError = TalError.None;
      return BlocksList.Count;
    }

    #endregion Protected Methods

    #region Constructors

    public EdfPlusAnnotationListBase(double blockDuration)
    {
      BlockDuration = blockDuration;
      AnnotationsList = new List<EdfPlusAnnotation>();
      BlocksList = new List<EdfPlusAnnotationDataBlock>();
      _formatInfo = TALConsts.ciEnglishUS;
    }

    #endregion Constructors

    #region Public Properties

    public int BlockCount { get { return GetBlockCount(); } }

    public double BlockDuration
    {
      get { return _blockDuration; }
      set
      {
        _lastError = TalError.None;
        if (_blockDuration != value)
        {
          // TODO -oMarco -cDevelopment : Add code to redistribute annotations
          _blockDuration = value;
        }
      }
    }

    public virtual bool BlocksContinuous
    {
      get
      {
        bool result = true;
        int i = 0;
        while (result && (i < BlockCount - 1))
        {
          result = (Block(i) != null) && (Block(i + 1) != null) &&
                   (MathEx.SameValue(Block(i).DataRecOnset + BlockDuration, Block(i + 1).DataRecOnset));
          i++;
        }
        return result;
      }
    }

    public virtual bool BlocksInOrder
    {
      get
      {
        bool result = true;
        int i = 0;
        double lastOnset = -1;
        while ((i < BlockCount) && (result))
        {
          if (Block(i) != null)
          {
            result = (Block(i).DataRecOnset > lastOnset);
            lastOnset = Block(i).DataRecOnset;
          }
          i++;
        }
        return result;
      }
    }

    public int Count
    {
      get
      {
        lock (AnnotationsList)
          return AnnotationsList.Count;
      }
    }

    public EdfPlusAnnotation Current { get { return _current; } }

    public HPDateTime FileStartDateTime
    {
      get { return _fileStartDateTime; }
      set
      {
        _lastError = TalError.None;
        if ((value.IsAssigned) && (_fileStartDateTime.IsAssigned) && (value == _fileStartDateTime))
          return;
        if ((!_fileStartDateTime.IsAssigned) || (BlockCount == 0))
          _fileStartDateTime = value;
        else
        {
          Debug.Assert(AllBlocksAvailable, TALConsts.NotAllTALBlocksAreAvailable);
          double dT = value.SecDifference(_fileStartDateTime);
          double block0Onset = Block(0).DataRecOnset;
          // If move filestart towards end, check if all annotations will be within a datablock
          if ((dT > 0) && (Count > 0) && (this[0].Onset < dT))
            throw new EdfPlusTALException(TALConsts.UnableToPerformRequestedOperation);
          // Check if there have to be any blocks removed
          if (BlockDuration > 0)
          {
            int index = -1;
            while ((index + 1 < BlockCount) && (Block(index + 1).DataRecOnset < dT))
              index++;
            if (index >= 0) //TODO: Check why don't lock the list here?
              BlocksList.RemoveRange(0, index + 1);
          }
          else
          {
            int index = 0;
            while ((index + 1 < BlockCount) && (Block(index + 1).DataRecOnset < dT))
              index++;
            if (index > 0)
              BlocksList.RemoveRange(1, index);
          }
          // Check the number of Blocks to be added
          int insertCount = 0;
          if (BlockDuration > 0)
          {
            if (BlocksContinuous)
            {
              if (BlockCount > 0)
                while ((dT + insertCount * BlockDuration) < (Block(0).DataRecOnset))
                  insertCount++;
              else
                insertCount = 1;
            }
            else
            {
              if ((BlockCount == 0) || !MathEx.SameValue(Block(0).DataRecOnset, Math.Abs(dT)))
                insertCount = 1;
            }
          }
          // Update Block Onset values
          if (BlockDuration > 0)
            for (int i = 0; i < BlockCount; i++)
              Block(i).DataRecOnset = Block(i).DataRecOnset - dT;
          else
            for (int i = 1; i < BlockCount; i++)
              Block(i).DataRecOnset = Block(i).DataRecOnset - dT;
          // Update Annotation Onset values
          for (int i = 0; i < Count; i++)
            this[i].Onset -= dT;
          // Insert new blocks
          for (int i = 0; i < insertCount; i++)
            BlocksList.Insert(0, null);
          for (int i = 0; i < insertCount; i++)
          {
            if (i == 0)
              AddBlock(i, block0Onset);
            else
              AddBlock(i, i * BlockDuration);
          }
          // Remove overlapping Block onsets
          if (BlockDuration > 0)
          {
            double tOnset = Block(0).DataRecOnset + BlockDuration;
            for (int i = 1; i < BlockCount; i++)
            {
              if (Block(i).DataRecOnset < tOnset)
                Block(i).DataRecOnset = tOnset;
              tOnset += BlockDuration;
            }
          }
          // Relink Annotations to updated Block Onset values
          RedistributeAnnotations();
          _fileStartDateTime = value;
        }
      }
    }

    public HPDateTime FirstAnnotationStart
    {
      get
      {
        return Count > 0 ? this[0].StartDateTime : FileStartDateTime;
      }
    }

    public IFormatProvider FormatInfo { get { return _formatInfo; } }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public HPDateTime LastAnnotationEnd
    {
      get
      {
        double tLast = double.MinValue;
        if (Count == 0)
          return FileStartDateTime;
        lock (AnnotationsList)
        {
          tLast = this.Aggregate(tLast, (current, annotation) => Math.Max(current, annotation.Onset + annotation.Duration));
        }
        return FileStartDateTime + tLast;
      }
    }

    public TalError LastError { get { return _lastError; } set { _lastError = value; } }

    public TALReadDataError OnTALReadDataError { get; set; }

    public double TFirstStart { get { return FirstAnnotationStart.SecDifference(FileStartDateTime); } }

    public EdfPlusAnnotation this[int index]
    {
      get
      {
        lock (AnnotationsList)
          return AnnotationsList[index];
      }
      set
      {
        lock (AnnotationsList)
          AnnotationsList[index] = value;
      }
    }

    public double TLastEnd { get { return LastAnnotationEnd.SecDifference(FileStartDateTime); } }

    public bool UpdateEnabled { get { return (_updateCount == 0); } }

    #endregion Public Properties

    #region Public Methods

    public void Add(EdfPlusAnnotation item)
    {
      Add(item, 0);
    }

    public int Add(double onset, string annotation)
    {
      return Add(onset, annotation, 0);
    }

    //public int Add(EDFPlusAnnotation annotation)
    //{
    //  return Add(annotation, 0);
    //}
    public int Add(EdfPlusAnnotation annotation, double addOnset)
    {
      return Add(annotation.Onset + addOnset, annotation.Duration, annotation.Annotation, annotation.AnnotationSignalNr);
    }

    public int Add(double onset, double duration, string annotation)
    {
      return Add(onset, duration, annotation, 0);
    }

    public int Add(double onset, string annotation, int annotationSignalNr)
    {
      return Add(onset, 0, annotation, annotationSignalNr);
    }

    public int Add(int blockNr, double onset, string annotation)
    {
      return Add(blockNr, onset, annotation, 0);
    }

    public int Add(double onset, double duration, string annotation, int annotationSignalNr)
    {
      _lastError = TalError.None;
      int index = GetBlockByOnset(onset);
      if (index >= 0)
        return Add(index, onset, duration, annotation, annotationSignalNr);
      _lastError = TalError.BlockNotFound;
      return -1;
    }

    public int Add(int blockNr, double onset, double duration, string annotation)
    {
      return Add(blockNr, onset, duration, annotation, 0);
    }

    public int Add(int blockNr, double onset, string annotation, int annotationSignalNr)
    {
      return Add(blockNr, onset, 0, annotation, annotationSignalNr);
    }

    public int Add(int blockNr, double onset, double duration, string annotation, int annotationSignalNr)
    {
      _lastError = TalError.None;
      try
      {
        int result;
        EdfPlusAnnotation newAnnotation = new EdfPlusAnnotation(this, blockNr, onset, duration, annotation, annotationSignalNr);
        lock (AnnotationsList)
        {
          AnnotationsList.Add(newAnnotation);
          result = AnnotationsList.IndexOf(newAnnotation);
        }
        Block(blockNr).Modified = true;
        if (UpdateEnabled)
          DoSortAnnotations();
        return result;
      }
      catch
      {
        _lastError = TalError.AddToList;
        return -1;
      }
    }

    public bool AddBlock(int blockNr, double onset)
    {
      Debug.Assert(blockNr >= 0, TALConsts.InvalidBlockIndex);
      Debug.Assert(double.IsNaN(onset) || (onset >= 0), TALConsts.InvalidBlockOnset);
      _lastError = TalError.None;
      DoSetBlocksListCapacity(blockNr);
      while (blockNr >= BlocksList.Count)
        BlocksList.Add(null);
      if (BlocksList[blockNr] != null)
        Block(blockNr).DataRecOnset = onset;
      else
      {
        EdfPlusAnnotationDataBlock newBlock = new EdfPlusAnnotationDataBlock(this, blockNr, onset) { Modified = true };
        BlocksList[blockNr] = newBlock;
      }
      return (Block(blockNr) != null);
    }

    public bool AddBlocks(int blockNr, double onset, int nrBlocks)
    {
      _lastError = TalError.None;
      Debug.Assert(blockNr >= 0, TALConsts.InvalidBlockIndex);
      Debug.Assert(onset >= 0, TALConsts.InvalidBlockOnset);
      Debug.Assert(BlockDuration > 0, TALConsts.InvalidBlockDuration);
      // Pre-add items to list to avoid multiple resizes of _blocksList
      DoSetBlocksListCapacity(blockNr + nrBlocks);
      bool result = true;
      int i = 0;
      while (result && (i < nrBlocks))
      {
        result = AddBlock(blockNr + i, onset + i * BlockDuration);
        i++;
      }
      return result;
    }

    public void BeginUpdate()
    {
      _updateCount++;
    }

    public EdfPlusAnnotationDataBlock Block(int index)
    {
      return GetBlock(index);
    }

    public bool BlockInMemory(int index)
    {
      Debug.Assert(index >= 0, TALConsts.InvalidBlockIndex);
      if (index < BlocksList.Count)
        return (BlocksList[index] != null);
      return false;
    }

    public virtual void CancelChanges()
    {
      Clear();
    }

    public void Clear()
    {
      lock (AnnotationsList)
        AnnotationsList.Clear();
      lock (BlocksList)
        BlocksList.Clear();
    }

    public bool Contains(EdfPlusAnnotation item)
    {
      lock (AnnotationsList)
        return AnnotationsList.Contains(item);
    }

    public void CopyTo(EdfPlusAnnotation[] array, int arrayIndex)
    {
      lock (AnnotationsList)
        AnnotationsList.CopyTo(array, arrayIndex);
    }

    public void EndUpdate()
    {
      if (_updateCount > 0)
      {
        _updateCount--;
        if (_updateCount == 0)
          DoSortAnnotations();
      }
    }

    public bool Extract(EdfPlusAnnotation annotation)
    {
      int blockIndex;
      bool result;
      lock (AnnotationsList)
      {
        blockIndex = annotation.DataRecNr;
        result = AnnotationsList.Remove(annotation);
      }
      if (result)
        Block(blockIndex).Modified = true;
      return result;
    }

    public int Find(EdfPlusAnnotation annotation)
    {
      return Find(annotation.Onset, annotation.Duration, annotation.Annotation, annotation.AnnotationSignalNr);
    }

    public int Find(double onset, double duration, string annotation, int annotationSignalNr)
    {
      int result = -1;
      int i = 0;
      while ((i < Count) && (result < 0))
      {
        if (MathEx.SameValue(this[i].Onset, onset) &&
            MathEx.SameValue(this[i].Duration, duration) &&
            (this[i].AnnotationSignalNr == annotationSignalNr) &&
            (this[i].Annotation.ToLower() == annotation.ToLower()))
          result = i;
        else
        {
          if (this[i].Onset > onset)
            i = Count;
          else
            i++;
        }
      }
      return result;
    }

    public virtual int GetBlockByOnset(double onset)
    {
      for (int i = 0; i < BlockCount; i++)
      {
        if (Block(i) != null)
          if (Block(i).DataRecOnset <= onset)
            return i;
      }
      return -1;
    }

    public virtual int GetBlockByOnset(HPDateTime onset)
    {
      return GetBlockByOnset(FileStartDateTime.SecDifference(onset));
    }

    public IEnumerator<EdfPlusAnnotation> GetEnumerator()
    {
      return AnnotationsList.GetEnumerator();
      //throw new NotImplementedException();
    }

    public int IndexOf(EdfPlusAnnotation item)
    {
      lock (AnnotationsList)
        return AnnotationsList.IndexOf(item);
    }

    public void Insert(int index, EdfPlusAnnotation item)
    {
      lock (AnnotationsList)
        AnnotationsList.Insert(index, item);
    }

    public int MinimalBufferSize(int annotationSignalNr = 0)
    {
      LastError = TalError.None;
      int result = 0;
      int i = 0;
      while ((i < BlockCount) && (result >= 0))
      {
        if (Block(i) != null)
        {
          result = Math.Max(result, Block(i).MinimalBufferSize(annotationSignalNr));
          if (Block(i).LastError != TalError.None)
          {
            LastError = Block(i).LastError;
            result = -1;
          }
        }
        i++;
      }
      return result;
    }

    public bool ReadFromBuffer(int blockNr, ref short[] buffer, int bufferOffset, int bufferSize)
    {
      return ReadFromBuffer(blockNr, ref buffer, bufferOffset, bufferSize, 0);
    }

    public bool ReadFromBuffer(int blockNr, ref short[] buffer, int bufferOffset, int bufferSize, int annotationSignalNr)
    {
      Debug.Assert(blockNr >= 0, TALConsts.InvalidBlockIndex);
      bool result = true;
      if (!BlockInMemory(blockNr))
        result = AddBlock(blockNr, double.NaN);
      if (result)
      {
        BeginUpdate();
        try
        {
          Block(blockNr).ReadFromBuffer(ref buffer, bufferOffset, bufferSize, annotationSignalNr);
        }
        finally
        {
          EndUpdate();
        }
      }
      LastError = Block(blockNr).LastError;
      return result;
    }

    public bool RedistributeAnnotations()
    {
      _lastError = TalError.None;
      lock (AnnotationsList)
      {
        foreach (var annotation in this)
        {
          int index = GetBlockByOnset(annotation.Onset);
          if (index >= 0)
            annotation.DataRecNr = GetBlockByOnset(Current.Onset);
          else
          {
            _lastError = TalError.BlockNotFound;
            break;
          }
        }
      }
      return (_lastError == TalError.None);
    }

    public bool Remove(EdfPlusAnnotation item)
    {
      lock (AnnotationsList)
      {
        int blockIndex = item.DataRecNr;
        Block(blockIndex).Modified = true;
        return AnnotationsList.Remove(item);
      }
    }

    public void RemoveAt(int index)
    {
      lock (AnnotationsList)
      {
        if ((index < 0) || (index >= Count))
          return;
        int blockIndex = AnnotationsList[index].DataRecNr;
        Block(blockIndex).Modified = true;
        AnnotationsList.RemoveAt(index);
      }
    }

    public void RemoveDuplicates(bool ignoreSignalNr)
    {
      BeginUpdate();
      try
      {
        int index = 0;
        int j = 1;
        while (index < Count - 1)
        {
          EdfPlusAnnotation a1 = this[index];
          EdfPlusAnnotation a2 = this[index + 1];
          if (MathEx.SameValue(a1.Onset, a2.Onset))
          {
            if (MathEx.SameValue(a1.Duration, a2.Duration) && (a1.Annotation.ToLower() == a2.Annotation.ToLower()) &&
                (ignoreSignalNr || (a1.AnnotationSignalNr == a2.AnnotationSignalNr)))
              RemoveAt(index + j);
            else
              j++;
          }
          else
          {
            index++;
            j = 1;
          }
          if ((index < Count - 1) && (index + j > Count - 1))
          {
            j = 1;
            index++;
          }
        }
      }
      finally
      {
        EndUpdate();
      }
    }

    public bool SelectFirst()
    {
      return SelectFirst(ref _current);
    }

    public bool SelectFirst(double onset)
    {
      return SelectFirst(onset, ref _current);
    }

    public virtual bool SelectFirst(ref EdfPlusAnnotation annotation)
    {
      _lastError = TalError.None;
      if (Count > 0)
      {
        annotation = this[0];
        return true;
      }
      return false;
    }

    public virtual bool SelectFirst(double onset, ref EdfPlusAnnotation annotation)
    {
      _lastError = TalError.None;
      // TODO: Check if the standard Find method can be used
      // Perform binary search
      int indexLeft = -1;
      int indexRight = Count;
      lock (AnnotationsList)
      {
        while (indexRight - indexLeft > 1)
        {
          int index = (indexLeft + indexRight) / 2;
          if (this[index].Onset < onset)
            indexLeft = index;
          else
            indexRight = index;
        }
      }
      // IdxRight now contains Index of first occurance of AOnset or where it should be
      if (indexRight < Count)
      {
        annotation = this[indexRight];
        return true;
      }
      return false;
    }

    public bool SelectLast()
    {
      return SelectLast(ref _current);
    }

    public virtual bool SelectLast(ref EdfPlusAnnotation annotation)
    {
      _lastError = TalError.None;
      if (Count > 0)
      {
        annotation = this[Count - 1];
        return true;
      }
      return false;
    }

    public bool SelectNext()
    {
      return SelectNext(ref _current);
    }

    public virtual bool SelectNext(ref EdfPlusAnnotation annotation)
    {
      _lastError = TalError.None;
      lock (AnnotationsList)
      {
        if (annotation != null)
        {
          int index = IndexOf(annotation);
          if (index < Count - 1)
          {
            annotation = this[index + 1];
            return true;
          }
        }
        else
          _lastError = TalError.EndOfList;
      }
      return false;
    }

    public bool SelectPrev()
    {
      return SelectPrev(ref _current);
    }

    public virtual bool SelectPrev(ref EdfPlusAnnotation annotation)
    {
      _lastError = TalError.None;
      lock (AnnotationsList)
      {
        if (annotation != null)
        {
          int index = IndexOf(annotation);
          if (index > 0)
          {
            annotation = this[index - 1];
            return true;
          }
        }
        else
          _lastError = TalError.StartOfList;
      }
      return false;
    }

    public void SetBlocksListCapacity(int nrBlocks)
    {
      DoSetBlocksListCapacity(nrBlocks, false);
    }

    public bool WriteToBuffer(int blockNr, ref short[] buffer, int bufferOffset, int bufferSize)
    {
      return WriteToBuffer(blockNr, ref buffer, bufferOffset, bufferSize, 0);
    }

    public bool WriteToBuffer(int blockNr, ref short[] buffer, int bufferOffset, int bufferSize, int annotationSignalNr)
    {
      Debug.Assert((blockNr >= 0) && (blockNr < BlockCount), TALConsts.InvalidBlockIndex);
      bool result = Block(blockNr).WriteToBuffer(ref buffer, bufferOffset, bufferSize, annotationSignalNr);
      LastError = Block(blockNr).LastError;
      return result;
    }

    #endregion Public Methods
  }
}