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
using System.IO;

namespace NeuroLoopGainLibrary.IO
{
  public class MinimalReadBufferSizeFileStream : FileStream
  {
    private int _minimalReadBufferSize;
    private object _readAccess;
    
    private void SetMinimimalReadBufferSize(int value)
    {
      _minimalReadBufferSize = value;
    }
    
    private object GetReadAccess()
    {
      return _readAccess ?? (_readAccess = new object());
    }

    protected int FileOffset;
    protected byte[] TempBuffer = new byte[1];
    protected object ReadAccess { get { return GetReadAccess(); } }
   
    public override int Read(byte[] array, int offset, int count)
    {
      lock (ReadAccess)
      {
        if ((_minimalReadBufferSize <= 0) || (count > _minimalReadBufferSize))
          return base.Read(array, offset, count);
        
        // First check if we have to read data from disk...
        if ((TempBuffer.Length == 0) || (Position < FileOffset) || (Position + count > FileOffset + TempBuffer.Length))
        {
          FileOffset = (int)Position;
          Array.Resize(ref TempBuffer, Math.Min(_minimalReadBufferSize, (int)(Length - Position + 1)));
          long oldPosition = Position;
          int nrBytesRead = base.Read(TempBuffer, 0, TempBuffer.Length);
          if (nrBytesRead < TempBuffer.Length)
            Array.Resize(ref TempBuffer, nrBytesRead);
          Seek(oldPosition, SeekOrigin.Begin);  
        }
        long bufferPos = Position - FileOffset;
        int result = Math.Min(count, TempBuffer.Length - (int)bufferPos);
        Array.Copy(TempBuffer, bufferPos, array, 0, result);
        Seek(result, SeekOrigin.Current);
        return result;
      }
    }

    public MinimalReadBufferSizeFileStream(string path, FileMode mode, FileAccess access)
      : base(path, mode, access)
    {
    }

    public MinimalReadBufferSizeFileStream(string path, FileMode mode, FileAccess access, FileShare share)
      : base(path, mode, access, share)
    {
    }
    
    public int MinimalReadBufferSize
    {
      get { return _minimalReadBufferSize; }
      set { SetMinimimalReadBufferSize(value); }
    }
  }
}