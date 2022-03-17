using Godot;
using System;

public static class Utils
{

	public static byte[] Data2ByteArray(Godot.Collections.Array data)
	{
		return GD.Var2Str(data).ToUTF8();
	}

	public static Godot.Collections.Array ByteArray2Data(byte[] byteArray)
	{
		string dataString = byteArray.GetStringFromUTF8();
		return GD.Str2Var(dataString) as Godot.Collections.Array;
	}
}
