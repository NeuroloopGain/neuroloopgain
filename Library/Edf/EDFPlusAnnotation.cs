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

using System.Diagnostics;
using NeuroLoopGainLibrary.DateTimeTypes;
using NeuroLoopGainLibrary.Mathematics;

namespace NeuroLoopGainLibrary.Edf
{
  public class EdfPlusAnnotation
  {


    #region Constructors
    public EdfPlusAnnotation(EdfPlusAnnotationListBase owner) : this(owner, 0, 0, string.Empty, -1) { }

    public EdfPlusAnnotation(EdfPlusAnnotationListBase owner, int dataRecNr, double onset, double duration, string annotation) :
      this(owner, dataRecNr, onset, duration, annotation, 0) { }

    public EdfPlusAnnotation(
      EdfPlusAnnotationListBase owner, int dataRecNr, double onset, double duration, string annotation, int annotationSignalNr)
    {
      FileOrder = -1;
      _owner = owner;
      DataRecNr = dataRecNr;
      _onset = onset;
      _duration = duration;
      _annotation = annotation;
      _annotationSignalNr = annotationSignalNr;
      Modified = false;
    }

    public EdfPlusAnnotation(EdfPlusAnnotationListBase owner, int dataRecNr, double onset, string annotation) :
      this(owner, dataRecNr, onset, annotation, 0) { }

    public EdfPlusAnnotation(
      EdfPlusAnnotationListBase owner, int dataRecNr, double onset, string annotation, int annotationSignalNr) :
      this(owner, dataRecNr, onset, 0, annotation, annotationSignalNr) { }
    #endregion

    #region Private members
    private string _annotation;
    private int _annotationSignalNr;
    private double _duration;
    private int _fileOrder = -1;
    private double _onset;
    private EdfPlusAnnotationListBase _owner;

    private int GetOwnerListIndex()
    {
      return Owner.IndexOf(this);
    }

    private HPDateTime GetStartDateTime()
    {
      return new HPDateTime(Owner.FileStartDateTime, Onset);
    }
    #endregion

    #region Properties
    public string Annotation
    {
      get { return _annotation; }
      set
      {
        if (_annotation == value)
          return;
        _annotation = value;
        Modified = true;
      }
    }

    public int AnnotationSignalNr
    {
      get { return _annotationSignalNr; }
      set
      {
        Debug.Assert(value >= 0, TALConsts.AnnotationSignalNrError);
        if (_annotationSignalNr != value)
        {
          _annotationSignalNr = value;
          Modified = true;
        }
      }
    }

    public int DataRecNr { get; set; }

    public double Duration
    {
      get { return _duration; }
      set
      {
        if (MathEx.SameValue(_duration, value)) return;
        _duration = value;
        Modified = true;
      }
    }

    public HPDateTime EndDateTime
    {
      get
      {
        return new HPDateTime(Owner.FileStartDateTime, Onset + Duration);
      }
      set
      {
        Duration = value.SecDifference(StartDateTime);
      }
    }

    public int FileOrder
    {
      get { return _fileOrder; }
      set { _fileOrder = value; }
    }

    public bool Modified { get; set; }

    public double Onset
    {
      get { return _onset; }
      set
      {
        if (MathEx.SameValue(_onset, value)) return;
        _onset = value;
        Modified = true;
      }
    }

    public EdfPlusAnnotationListBase Owner
    {
      get { return _owner; }
      set
      {
        if (_owner != value)
        {
          _owner = value;
          Modified = true;
        }
      }
    }

    public int OwnerListIndex { get { return GetOwnerListIndex(); } }

    public HPDateTime StartDateTime
    {
      get
      {
        return new HPDateTime(Owner.FileStartDateTime, Onset);
      }
      set
      {
        Onset = value.SecDifference(StartDateTime);
      }
    }
    #endregion

    #region Public members
    public override string ToString()
    {
      if (Duration > 0)
      {
        return string.Format("{0}{1}{2}{3}{4}{5}{6}",
                             Onset >= 0 ? "+" : string.Empty,
                             Onset.ToString("G", TALConsts.ciEnglishUS),
                             TALConsts.c21,
                             Duration.ToString("G", TALConsts.ciEnglishUS),
                             TALConsts.c20, Annotation, TALConsts.c20);
      }
      return string.Format("{0}{1}{2}{3}{4}",
                           Onset >= 0 ? "+" : string.Empty,
                           Onset.ToString("G", TALConsts.ciEnglishUS),
                           TALConsts.c20, Annotation, TALConsts.c20);
    }

    #endregion

    #region Public static members

    public static int CompareEdfPlusAnnotation(EdfPlusAnnotation item1, EdfPlusAnnotation item2)
    {
      if (MathEx.SameValue(item1.Onset, item2.Onset))
        return AnnotationTagSort(item1.FileOrder, item2.FileOrder);
      if (item1.Onset < item2.Onset)
        return -1;
      return 1;
    }

    public static int AnnotationTagSort(int tag1, int tag2)
    {
      if (tag1 >= 0)
      {
        if (tag2 >= 0)
        {
          if (tag1 < tag2)
            return -1;
          if (tag1 > tag2)
            return 1;
          return 0;
        }
        return -1;
      }
      if (tag2 >= 0)
        return 1;
      return 0;
    }

    #endregion
  }
}