using UnityEngine;

namespace GTAI.TaskSystem
{
	public class SharedVariable<T>
	{
		public string Name { get; set; }

		public T Value { get; set; }
		
		public SharedVariable() { }

		public SharedVariable(T value)
		{
			Value = value;
		}

		/// <summary>
		/// This allows us to assign a value of type T to a shared variable object without going through a constructor.
		/// This has to be defined in each child class below.
		/// </summary>
		public static implicit operator SharedVariable<T>(T value)
		{
			return new SharedVariable<T>(value);
		}
	}

	public class SharedBool : SharedVariable<bool>
	{
		public static implicit operator SharedBool(bool value)
		{
			return new SharedBool { Value = value };
		}
	}

	public class SharedFloat : SharedVariable<float>
	{
		public static implicit operator SharedFloat(float value)
		{
			return new SharedFloat { Value = value };
		}
	}

	public class SharedInt : SharedVariable<int>
	{
		public static implicit operator SharedInt(int value)
		{
			return new SharedInt { Value = value };
		}
	}

	public class SharedString : SharedVariable<string>
	{
		public static implicit operator SharedString(string value)
		{
			return new SharedString { Value = value };
		}
	}

	public class SharedVector3 : SharedVariable<Vector3>
	{
		public static implicit operator SharedVector3(Vector3 value)
		{
			return new SharedVector3 { Value = value };
		}
	}

	public class SharedVector2 : SharedVariable<Vector2>
	{
		public static implicit operator SharedVector2(Vector2 value)
		{
			return new SharedVector2 { Value = value };
		}
	}

	public class SharedTransform : SharedVariable<Transform>
	{
		public static implicit operator SharedTransform(Transform value)
		{
			return new SharedTransform { Value = value };
		}
	}

	public class SharedGameObject : SharedVariable<GameObject>
	{
		public static implicit operator SharedGameObject(GameObject value)
		{
			return new SharedGameObject { Value = value };
		}
	}

	public class SharedColor : SharedVariable<Color>
	{
		public static implicit operator SharedColor(Color value)
		{
			return new SharedColor { Value = value };
		}
	}
}