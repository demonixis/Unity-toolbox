using System.Collections;
using UnityEngine;

namespace Demonixis.Toolbox.Utils
{
    public static class CoroutineHelper
    {
        public static IEnumerator UnscaledWaitForSeconds(float time)
        {
            var start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < start + time)
                yield return null;
        }
    }
}
