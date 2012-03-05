namespace Gour.Phone.Push
{
    using System;

    public interface ISamplePushUserRegistrationClient
    {
        void Connect(Action<SamplePushUserRegistrationResponse<string>> callback);

        void Disconnect(Action<SamplePushUserRegistrationResponse<string>> callback);

        void GetUpdates(Action<SamplePushUserRegistrationResponse<string[]>> callback);
    }
}
