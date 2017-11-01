using System;

namespace UnityEngine.UI
{
	public class UIItemDatabase : ScriptableObject {
	
		public UIItemInfo[] items;
		
		/// <summary>
		/// Get the specified ItemInfo by index.
		/// </summary>
		/// <param name="index">Index.</param>
		public UIItemInfo Get(int index)
		{
			return (this.items[index]);
		}
		
		/// <summary>
		/// Gets the specified ItemInfo by ID.
		/// </summary>
		/// <returns>The ItemInfo or NULL if not found.</returns>
		/// <param name="ID">The item ID.</param>
		public UIItemInfo GetByID(int ID)
		{
			for (int i = 0; i < this.items.Length; i++)
			{
				if (this.items[i].ID == ID)
					return this.items[i];
			}
			
			return null;
		}

        /// <summary>
        /// Adds new item to the database.
        /// </summary>
        /// <param name="item"></param>
        public void Add(UIItemInfo item)
        {
            // Get current size
            int size = this.items.Length;

            // Temporary array with increased size
            UIItemInfo[] newItems = new UIItemInfo[size + 1];

            // Copy the items to the new array
            this.items.CopyTo(newItems, 0);

            // Assign the new item
            newItems[size] = item;

            // Assign the new array
            this.items = newItems;
        }
	}
}
