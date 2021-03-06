﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NeuroLoopGain {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NeuroLoopGain.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select an EDF file to analyze.
        /// </summary>
        internal static string CaptionSelectEdfFileToAnalyze {
            get {
                return ResourceManager.GetString("CaptionSelectEdfFileToAnalyze", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select an output EDF file.
        /// </summary>
        internal static string CaptionSelectOutputEdfFile {
            get {
                return ResourceManager.GetString("CaptionSelectOutputEdfFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Artifact thresholds or -spread incorrect.
        /// </summary>
        internal static string ErrorArtifactsThresholds {
            get {
                return ResourceManager.GetString("ErrorArtifactsThresholds", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MCEventDuration = {0} but should be &gt;= 1.
        /// </summary>
        internal static string ErrorMCEventDurationTooSmall {
            get {
                return ResourceManager.GetString("ErrorMCEventDurationTooSmall", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MCgain = {0} but should be &gt;= 1.0.
        /// </summary>
        internal static string ErrorMCGainTooSmall {
            get {
                return ResourceManager.GetString("ErrorMCGainTooSmall", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to piB = {0} is smaller than LogFloat_Y0*10.
        /// </summary>
        internal static string ErrorpiBTooSmall {
            get {
                return ResourceManager.GetString("ErrorpiBTooSmall", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SmoothRate = {0} but should be &gt; 0.0 and &lt; 1.0).
        /// </summary>
        internal static string ErrorSmoothrateOutOfRange {
            get {
                return ResourceManager.GetString("ErrorSmoothrateOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The underSampled frequency should be &gt;= 1 Hz.
        /// </summary>
        internal static string ErrorUndersampleFrequencyTooSmall {
            get {
                return ResourceManager.GetString("ErrorUndersampleFrequencyTooSmall", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Computing PiB value....
        /// </summary>
        internal static string MessageComputePiB {
            get {
                return ResourceManager.GetString("MessageComputePiB", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Computing final gains....
        /// </summary>
        internal static string MessageComputingFinalGains {
            get {
                return ResourceManager.GetString("MessageComputingFinalGains", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Detecting artifacts....
        /// </summary>
        internal static string MessageDetectArtifacts {
            get {
                return ResourceManager.GetString("MessageDetectArtifacts", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Detecting events....
        /// </summary>
        internal static string MessageDetectingEvents {
            get {
                return ResourceManager.GetString("MessageDetectingEvents", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Performing SU and SS reduction....
        /// </summary>
        internal static string MessagePerformingSUSSReduction {
            get {
                return ResourceManager.GetString("MessagePerformingSUSSReduction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reading input signal....
        /// </summary>
        internal static string MessageReadingInputSignal {
            get {
                return ResourceManager.GetString("MessageReadingInputSignal", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Re-smoothing signals and detecting jumps....
        /// </summary>
        internal static string MessageResmoothingSUSS {
            get {
                return ResourceManager.GetString("MessageResmoothingSUSS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Smoothing SU and SS....
        /// </summary>
        internal static string MessageSmoothingSUSS {
            get {
                return ResourceManager.GetString("MessageSmoothingSUSS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Usage information.
        /// </summary>
        internal static string UsageInformation {
            get {
                return ResourceManager.GetString("UsageInformation", resourceCulture);
            }
        }
    }
}
