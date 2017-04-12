using System;
using System.Collections.Generic;

namespace Common.Utilities.Telemetry
{
    public interface ITelemetry
    {
        /// <summary>
        /// Tracks pages, screens, blades or forms
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="duration"></param>
        void TrackPageView(string pageName, TimeSpan duration);

        /// <summary>
        /// Tracks user actions and other events, user behavior or to monitor performance.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="properties"></param>
        /// <param name="metrics"></param>
        void TrackEvent(string eventName, Dictionary<string, string> properties = null, Dictionary<string, double> metrics = null);

        /// <summary>
        /// Track performance measurements such as queue lengths not related to specific events
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="properties"></param>
        void TrackMetric(string name, double value, Dictionary<string, string> properties = null);

        /// <summary>
        /// Traces exceptions for diagnosis. Traces where they occur in relation to other events and examine stack traces.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="handled"></param>
        /// <param name="properties"></param>
        /// <param name="metrics"></param>
        void TrackException(Exception exception, bool handled, Dictionary<string, string> properties = null, Dictionary<string, double> metrics = null);

        /// <summary>
        /// Tracks the frequency and duration of server requests for performance analysis.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startTime"></param>
        /// <param name="duration"></param>
        /// <param name="responseCode"></param>
        /// <param name="success"></param>
        void TrackRequest(string name, DateTimeOffset startTime, TimeSpan duration, string responseCode, bool success);

        /// <summary>
        /// Manually flushes all the data to server
        /// </summary>
        void Flush();
    }
}
