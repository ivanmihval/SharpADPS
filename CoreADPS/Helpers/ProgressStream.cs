using System;
using System.IO;

namespace CoreADPS.Helpers
{
    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(ulong position, ulong length)
        {
            Position = position;
            Length = length;
        }

        public ulong Position { get; private set; }
        public ulong Length { get; private set; }
    }

    // https://stackoverflow.com/a/57439154
    public class ProgressStream : Stream
    {
        private readonly Stream _mInput;
        private readonly ulong _mLength;
        private ulong _mPosition;
        public event EventHandler<ProgressEventArgs> UpdateProgress;

        public ProgressStream(Stream input)
        {
            _mInput = input;
            _mLength = (ulong) input.Length;
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int n = _mInput.Read(buffer, offset, count);
            _mPosition += (ulong) n;

            if (UpdateProgress != null)
            {
                UpdateProgress.Invoke(this, new ProgressEventArgs(_mPosition, _mLength));
            }
            
            return n;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return (long) _mLength; }
        }

        public override long Position
        {
            get { return (long) _mPosition; }
            set { throw new NotImplementedException(); }
        }
    }
}
