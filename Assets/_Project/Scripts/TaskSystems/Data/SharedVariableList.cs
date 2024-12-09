using System.Collections.Generic;
using System.Linq;

namespace GTAI.TaskSystem
{
	public class SharedVariableList<T>
	{
		public IReadOnlyList<SharedVariable<T>> List => _list.AsReadOnly();
		private readonly List<SharedVariable<T>> _list = new();

		public void Add(SharedVariable<T> v)
		{
			_list.Add(v);
		}

		public void Remove(SharedVariable<T> v)
		{
			_list.Remove(v);
		}

		public SharedVariable<T> Get(string name)
		{
			return _list.FirstOrDefault(v => v.Name == name);
		}
	}
}