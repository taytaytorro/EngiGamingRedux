using System;
using BepInEx.Logging;

namespace EngiShotgun
{
	// Token: 0x02000006 RID: 6
	internal static class Log
	{
		// Token: 0x0600001D RID: 29 RVA: 0x00002DA4 File Offset: 0x00000FA4
		internal static void Init(ManualLogSource logSource)
		{
			_logSource = logSource;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002DAD File Offset: 0x00000FAD
		internal static void LogDebug(object data)
		{
			_logSource.LogDebug(data);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002DBB File Offset: 0x00000FBB
		internal static void LogError(object data)
		{
			_logSource.LogError(data);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002DC9 File Offset: 0x00000FC9
		internal static void LogFatal(object data)
		{
			_logSource.LogFatal(data);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002DD7 File Offset: 0x00000FD7
		internal static void LogInfo(object data)
		{
			_logSource.LogInfo(data);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002DE5 File Offset: 0x00000FE5
		internal static void LogMessage(object data)
		{
			_logSource.LogMessage(data);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002DF3 File Offset: 0x00000FF3
		internal static void LogWarning(object data)
		{
			_logSource.LogWarning(data);
		}

		// Token: 0x0400002A RID: 42
		internal static ManualLogSource _logSource;
	}
}
