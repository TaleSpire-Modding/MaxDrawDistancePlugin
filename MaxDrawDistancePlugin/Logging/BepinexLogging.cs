using BepInEx.Logging;
using Sentry;
using System;

namespace MaxDrawDistance.Logging
{
    public abstract class BepinexLogging
    {
        protected ManualLogSource Logger { get; }

        protected BepinexLogging()
        {
            string LoggerID = GetType().AssemblyQualifiedName;
            Logger = BepInEx.Logging.Logger.CreateLogSource(LoggerID);
        }

        /// <summary>
        /// Generates a log forwarding function for Sentry.
        /// </summary>
        public static EventHandler<LogEventArgs> GenerateSentryLogFowarding(SentryOptions sentryOptions, string pluginVersion)
        {
            void output(object sender, LogEventArgs e)
            {
                void _scope(Scope scope)
                {
                    scope.User = new User
                    {
                        Username = BackendManager.TSUserID.Value.ToString(),
                    };
                    scope.Release = pluginVersion;
                }

                using (SentrySdk.Init(sentryOptions))
                {
                    switch (e.Level)
                    {
                        case LogLevel.Fatal:
                            SentrySdk.CaptureMessage(e.Data.ToString(), _scope, SentryLevel.Fatal);
                            break;
                        case LogLevel.Error:
                            SentrySdk.CaptureMessage(e.Data.ToString(), _scope, SentryLevel.Error);
                            break;
                        case LogLevel.Warning:
                            SentrySdk.CaptureMessage(e.Data.ToString(), _scope, SentryLevel.Warning);
                            break;

                        default:
                            // Only want to log the following levels to Sentry if we're in debug mode
                            if (pluginVersion == "0.0" + ".0.0")
                            {
                                switch (e.Level)
                                {
                                    case LogLevel.Info:
                                    case LogLevel.Message:
                                        SentrySdk.CaptureMessage(e.Data.ToString(), _scope, SentryLevel.Info);
                                        break;
                                    case LogLevel.Debug:
                                        SentrySdk.CaptureMessage(e.Data.ToString(), _scope, SentryLevel.Debug);
                                        break;
                                }
                            }
                            break;
                    }
                }
            }

            return output;
        }
    }
}
