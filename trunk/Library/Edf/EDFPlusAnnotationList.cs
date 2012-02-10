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
using System.Diagnostics;
using NeuroLoopGainLibrary.Mathematics;

namespace NeuroLoopGainLibrary.Edf
{
  public class EdfPlusAnnotationList : EdfPlusAnnotationListBase
  {


    #region Constructors
    public EdfPlusAnnotationList(double blockDuration) : base(blockDuration) { }
    #endregion

    #region Private members
    private bool _getBlockBusy;

    #endregion

    #region Protected members
    protected bool DoReadBlockDataFromFile(int blockNr)
    {
      Debug.Assert((blockNr >= 0) && (blockNr < BlockCount), TALConsts.InvalidBlockIndex);
      LastError = TalError.None;
      if (!_getBlockBusy && BlockInMemory(blockNr))
        return true;
      bool result = false;
      if (OnTALGetData != null)
        OnTALGetData(this, blockNr, ref result);
      return result;
    }

    protected override EdfPlusAnnotationDataBlock GetBlock(int index)
    {
      lock (BlocksList)
      {
        EdfPlusAnnotationDataBlock result = base.GetBlock(index);
        if (result == null)
        {
          _getBlockBusy = true;
          try
          {
            if (DoReadBlockDataFromFile(index))
              result = base.GetBlock(index);
          }
          finally
          {
            _getBlockBusy = false;
          }
        }
        return result;
      }
    }

    protected override int GetBlockCount()
    {
      LastError = TalError.None;
      int result = base.GetBlockCount();
      bool dummy = false;
      if (OnTALGetData != null) // Get Block -1 => block not available, but returns NrOfDataBlocks available
        result = Math.Max(OnTALGetData(this, -1, ref dummy), base.GetBlockCount());
      return result;
    }
    #endregion

    #region Public members
    public void AddBookMark()
    {
      DoAddBookMark();
    }

    public override void CancelChanges()
    {
      lock(AnnotationsList) 
        AnnotationsList.Clear();
      _getBlockBusy = true;
      try
      {
        for (int i = 0; i < BlockCount-1; i++)
        {
          if (Block(i) != null)
            DoReadBlockDataFromFile(i);
        }
      }
      finally
      {
        _getBlockBusy = false;
      }
    }

    public override int GetBlockByOnset(double onset)
    {
      int i = 0;
      int result = -1;
      while ((result < 0) && (i < BlockCount))
      {
        if ((Block(i) != null) || (DoReadBlockDataFromFile(i)))
        {
          if (BlockDuration > 0)
          {
            if (Block(i).Contains(onset))
              result = i;
            else
            {
              if (Block(i).DataRecOnset > onset)
              {
                if (i > 0)
                  result = i - 1;
                else
                  result = 0;
              }
              else
                if (i == BlockCount - 1)
                  result = i;
            }
          }
          else
          {
            if (Block(i).DataRecOnset > onset)
            {
              if (i > 0)
                result = i - 1;
              else
                result = 0;
            }
            else
              if (i == BlockCount - 1)
                result = i;
          }
        }
        i++;
      }
      return result;
    }

    public void ReleaseBookMark()
    {
      DoReleaseBookMark();
    }

    public override bool SelectFirst(double onset, ref EdfPlusAnnotation annotation)
    {
      LastError = TalError.None;
      GetBlockByOnset(onset);  // Automatically reads data from file if not available
      return base.SelectFirst(onset, ref annotation);
    }

    public override bool SelectFirst(ref EdfPlusAnnotation annotation)
    {
      LastError = TalError.None;
      if ((BlockCount > 0) && (Block(0) == null))
        DoReadBlockDataFromFile(0);
      return base.SelectFirst(ref annotation);
    }

    public override bool SelectLast(ref EdfPlusAnnotation annotation)
    {
      LastError = TalError.None;
      if ((BlockCount > 0) && (Block(BlockCount - 1) == null))
        DoReadBlockDataFromFile(BlockCount - 1);
      return base.SelectLast(ref annotation);
    }

    public override bool SelectNext(ref EdfPlusAnnotation annotation)
    {
      LastError = TalError.None;
      if ((annotation != null) && (annotation.DataRecNr >= 0) && (annotation.DataRecNr < BlockCount - 1) &&
          ((Block(annotation.DataRecNr + 1) == null) || !Block(annotation.DataRecNr + 1).DataReadFromBuffer))
        DoReadBlockDataFromFile(annotation.DataRecNr + 1);
      return base.SelectNext(ref annotation);
    }

    public override bool SelectPrev(ref EdfPlusAnnotation annotation)
    {
      LastError = TalError.None;
      if ((annotation != null) && (annotation.DataRecNr > 0) && (Block(annotation.DataRecNr - 1) == null))
        DoReadBlockDataFromFile(annotation.DataRecNr - 1);
      return base.SelectPrev(ref annotation);
    }
    #endregion

    #region Properties
    public new bool AllBlocksAvailable
    {
      get { return base.AllBlocksAvailable; }
    }

    public override bool BlocksContinuous
    {
      get
      {
        bool result = true;
        int i = 0;
        while (result && (i < BlockCount - 1))
        {
          if (Block(i) == null)
            DoReadBlockDataFromFile(i);
          if (Block(i + 1) == null)
            DoReadBlockDataFromFile(i + 1);
          result = (Block(i) != null) && (Block(i + 1) != null) && 
                   MathEx.SameValue(Block(i).DataRecOnset + BlockDuration, Block(i + 1).DataRecOnset);
          i++;
        }
        return result;
      }
    }

    public override bool BlocksInOrder
    {
      get
      {
        bool result = true;
        int i = 0;
        double lastOnset = double.NaN;
        while ((i < BlockCount) && result)
        {
          if ((Block(i) != null) || DoReadBlockDataFromFile(i))
          {
            if(!double.IsNaN(lastOnset))
              result = (Block(i).DataRecOnset >= lastOnset + BlockDuration);
            lastOnset = Block(i).DataRecOnset;
          }
          i++;
        }
        return result;
      }
    }

    //public List<EDFPlusAnnotation> FindByRegEx
    #endregion

    #region Events

    public TALGetData OnTALGetData { get; set; }

    #endregion
  }
}