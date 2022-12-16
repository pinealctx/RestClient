using System.Collections;
using UnityEngine;

namespace PinealCtx.RestClient.Tests
{
    public class Test
    {
        public IEnumerator TestA()
        {
            var client = new RestClient();
            yield return client.Get("https://www.google.com", Debug.Log);
        }
    }
}