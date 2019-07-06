using System;
using System.Collections;
using System.Text;

namespace Obi.Events.Audio.VuMeter
{
    //public delegate void PeakOverloadHandler(object sender, PeakOverloadEventArgs e);

    ///// <summary>
    ///// Event raised by the Vu meter when the signal level overloads.
    ///// </summary>
    //public class PeakOverloadEventArgs: EventArgs 
    //{
    //    private int mChannel;          // channel which overloaded
    //    private long mBytePosition;    // position where the overload happened (from start of recording)
    //    private double mTimePosition;  // time when the overload happened (from start of recording)

    //    public int Channel
    //    {
    //        get
    //        {
    //            return mChannel;
    //        }
    //    }

    //    public long BytePosition
    //    {
    //        get
    //        {
    //            return mBytePosition;
    //        }
    //    }

    //    public double TimePosition
    //    {
    //        get
    //        {
    //            return mTimePosition;
    //        }
    //    }

    //    /// <summary>
    //    /// Create a new PeakOverload event for a given byte position.
    //    /// </summary>
    //    /// <param name="channel">The channel that overloaded.</param>
    //    /// <param name="bytePosition">The position where the event occurred.</param>
    //    /// 
    //    // timePosition is added in same constructor by app team, India
    //    public PeakOverloadEventArgs(int channel, long bytePosition, double timePosition)
    //    {
    //        mChannel = channel;
    //        mBytePosition = bytePosition;
    //        mTimePosition = timePosition;
    //    }

    //    /// <summary>
    //    /// Create a new PeakOverload event at a given time.
    //    /// </summary>
    //    /// <param name="channel">The channel that overloaded.</param>
    //    /// <param name="timePosition">The time when the event occurred.</param>
    //    public PeakOverloadEventArgs(int channel, double timePosition)
    //    {
    //        mChannel = channel;
    //        mBytePosition = 0;
    //        mTimePosition = timePosition;
    //    }       
    //}
}
