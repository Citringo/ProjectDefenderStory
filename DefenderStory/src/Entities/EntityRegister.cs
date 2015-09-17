﻿
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace DefenderStory.Entities
{
	/// <summary>
	/// EntityRegister によって自動で登録される Entity クラスを指定します。このクラスは継承できません。
	/// </summary>
	[System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class EntityRegistryAttribute : Attribute
	{
		public string Name;
		public int ID;

		public EntityRegistryAttribute(string name, int id)
		{
			Name = name;
			ID = id;
		}


	}

	/// <summary>
	/// エンティティの情報を表します。
	/// </summary>
	public class EntityData
	{
		/// <summary>
		/// この Entity を表す Type。
		/// </summary>
		public Type EntityType;
		/// <summary>
		/// この Entity の一般名。
		/// </summary>
		public string EntityName;
		/// <summary>
		/// この Entity の ID。
		/// </summary>
		public int EntityID;
	}
		
	/// <summary>
	/// Entity の情報はここに保存され、ゲーム内で使用されます。
	/// </summary>
	public class EntityRegister : ICollection<EntityData>
	{

		public List<EntityData> Items { get; protected set; }

		public EntityRegister()
		{
			Items = new List<EntityData>();
			foreach (Type t in Assembly.GetExecutingAssembly().GetExportedTypes())
			{
				object[] attr = t.GetCustomAttributes(typeof(EntityRegistryAttribute), false);
				if (attr.Length > 0)
				{
					var era = (EntityRegistryAttribute)attr[0];
					this.Add(t, era.Name, era.ID);
				}
			}
		}

		public Entity CreateEntity(string name, PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			var data = GetDataByName(name);
			if (data == null)
				throw new InvalidOperationException(name + " というエンティティは存在しません");
			var o = Activator.CreateInstance(data.EntityType, pnt, obj, chips, par);

			if (!(o != null && o is Entity))
				throw new InvalidOperationException("Entity が存在しません。");

			return (Entity)o;
		}

		public Entity CreateEntity(string name, PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par, dynamic jsonobj)
		{
			Entity e = CreateEntity(name, pnt, obj, chips, par);
			if (jsonobj == null)
				return e;
			e.SetEntityData(jsonobj);

			return e;
		}

		public Entity CreateEntity(int id, PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			var data = this[id];
			if (data == null)
				throw new InvalidOperationException("" + id + " 番のエンティティは存在しません");
			var o = Activator.CreateInstance(data.EntityType, pnt, obj, chips, par);

			if (!(o != null && o is Entity))
				throw new InvalidOperationException("Entity が存在しません。");

			return (Entity)o;
		}

		public Entity CreateEntity(int id, PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par, dynamic jsonobj)
		{
			Entity e = CreateEntity(id, pnt, obj, chips, par);
			if (jsonobj == null)
				return e;
			e.SetEntityData(jsonobj);

			return e;
		}


		public void Add(EntityData item)
		{
			if (item.EntityID != -1 && GetDataById(item.EntityID) != null)
				throw new InvalidOperationException("既に同じ ID のエンティティが登録されています。");

			if (Contains(item) == false)
			{
				Items.Add(item);
			}

		}

		public void Add(Type entitytype, string name, int id)
		{
			if (id != -1 && GetDataById(id) != null)
				throw new InvalidOperationException("既に同じ ID のエンティティが登録されています。");
			var item = new EntityData
			{
				EntityID = id,
				EntityType = entitytype,
				EntityName = name
			};

			if (Contains(item) == false)
			{
				Items.Add(item);
			}

		}

		public EntityData GetDataById(int id)
		{
			foreach (var d in this.Where(new Func<EntityData, bool>((EntityData et) => et.EntityID == id)))
			{
				return d;
			}
			return null;
		}

		public EntityData GetDataByName(string name)
		{
			foreach (var d in this.Where(new Func<EntityData, bool>((EntityData et) => et.EntityName == name)))
			{
				return d;
			}
			return null;
		}

		public EntityData this[int i]
		{
			get
			{
				return GetDataById(i);
			}
		}


		public bool IsReadOnly { get { return false; } }
		public void Clear() { Items.Clear(); }
		public int Count { get { return Items.Count; } }
		public bool Contains(EntityData item) { return Items.Contains(item); }
		public void CopyTo(EntityData[] array, int arrayIndex) { Items.CopyTo(array, arrayIndex); }
		public bool Remove(EntityData item) { return Items.Remove(item); }
		public IEnumerator<EntityData> GetEnumerator()
		{
			return Items.GetEnumerator();

		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Items.GetEnumerator();
		}



	}


}