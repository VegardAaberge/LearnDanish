using System;
using Prism.Events;
using SpeakDanish.Domain.Models;

namespace SpeakDanish.Helpers
{
	public static class AppEvents
	{
        public class RecordingSelectedEvent : PubSubEvent<Recording> { }

        public static RecordingSelectedEvent RecordingSelected = new RecordingSelectedEvent();
    }
}

