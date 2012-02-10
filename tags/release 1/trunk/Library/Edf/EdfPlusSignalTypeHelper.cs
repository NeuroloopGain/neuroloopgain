using System;
using System.Collections.Generic;

namespace NeuroLoopGainLibrary.Edf
{
  public static class EdfPlusSignalTypeHelper
  {
    #region private fields

    private static List<string> _allUniqueEdfPlusSignalTypeBaseDim;

    #endregion private fields

    #region public fields

    /// <summary>
    /// Default base dimension that are compatible with the EDF+ signal types
    /// </summary>
    public static string[,] EdfPlusSignalTypeBaseDim = new[,]
                                                         {
                                                           {"m", string.Empty, string.Empty},
                                                           {"m^2", string.Empty, string.Empty},
                                                           {"m^3", string.Empty, string.Empty},
                                                           {"s", string.Empty, string.Empty},
                                                           {"m/s", string.Empty, string.Empty},
                                                           {"g", string.Empty, string.Empty},
                                                           {"rad", "deg", string.Empty},
                                                           {"%", string.Empty, string.Empty},
                                                           {string.Empty, string.Empty, string.Empty},
                                                           {"V", string.Empty, string.Empty},
                                                           {"V", string.Empty, string.Empty},
                                                           {"V", string.Empty, string.Empty},
                                                           {"V", string.Empty, string.Empty},
                                                           {"V", string.Empty, string.Empty},
                                                           {"V", string.Empty, string.Empty},
                                                           {"V", string.Empty, string.Empty},
                                                           {"V", string.Empty, string.Empty},
                                                           {"K", "degC", "degF"},
                                                           {string.Empty, string.Empty, string.Empty},
                                                           {"%", string.Empty, string.Empty},
                                                           {string.Empty, string.Empty, string.Empty},
                                                           {string.Empty, string.Empty, string.Empty},
                                                           {string.Empty, string.Empty, string.Empty},
                                                           {string.Empty, string.Empty, string.Empty}
                                                         };
    /// <summary>
    /// EDF+ signaltype identifier, used by signal labels to identify the type of signal.
    /// </summary>
    public static string[] EdfPlusSignalTypeNames = new[]
                                                      {
                                                        "Dist", "Area", "Vol", "Dur", "Vel", "Mass", "Angle", "%",
                                                        "Value", "EEG", "ECG", "EOG", "ERG", "EMG", "MEG", "MCG", "EP",
                                                        "Temp", "Resp", "SaO2", "Light", "Sound", "Event", "Unknown"
                                                      };

    #endregion public fields

    #region public properties

    /// <summary>
    /// Gets a list containing all unique edf plus signal type base dimensions.
    /// </summary>
    public static List<string> AllUniqueEdfPlusSignalTypeBaseDim
    {
      get
      {
        if (_allUniqueEdfPlusSignalTypeBaseDim == null)
        {
          _allUniqueEdfPlusSignalTypeBaseDim = new List<string>();
          for (int i = 0; i <= EdfPlusSignalTypeBaseDim.GetUpperBound(0); i++)
            for (int j = 0; j <= EdfPlusSignalTypeBaseDim.GetUpperBound(1); j++)
            {
              if (EdfPlusSignalTypeBaseDim[i, j] == string.Empty) continue;
              if (!_allUniqueEdfPlusSignalTypeBaseDim.Contains(EdfPlusSignalTypeBaseDim[i, j]))
                _allUniqueEdfPlusSignalTypeBaseDim.Add(EdfPlusSignalTypeBaseDim[i, j]);
            }
        }
        return _allUniqueEdfPlusSignalTypeBaseDim;
      }
    }

    #endregion public properties

    #region public methods

    /// <summary>
    /// Gets the name of the edf plus signal type.
    /// </summary>
    /// <param name="signalType">Type of the signal.</param>
    /// <returns>The name of the signal type.</returns>
    public static string GetEdfPlusSignalTypeName(EdfPlusSignalType signalType)
    {
      int i = Convert.ToInt32(signalType);
      return EdfPlusSignalTypeNames[i];
    }

    #endregion public methods
  }
}