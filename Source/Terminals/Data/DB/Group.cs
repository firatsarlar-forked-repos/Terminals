﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Terminals.Data.DB
{
    internal partial class Group : IGroup, IIntegerKeyEnityty
    {
        /// <summary>
        /// Gets or sets the virtual unique identifer. This isnt used, because of internal database identifier.
        /// Only for compatibility with file persistence.
        /// </summary>
        public IGroup Parent
        {
            get
            {
                return this.ParentGroup;
            }
            set
            {
                // todo ask cache in Groups, dont ask database directly
                using (var database = Database.CreateInstance())
                {
                    database.Attach(this);
                    this.ParentGroup = value as Group;
                    database.SaveImmediatelyIfRequested();
                    database.Detach(this);
                }
            }   
        }

        List<IFavorite> IGroup.Favorites
        {
            get
            {
                return this.Favorites.ToList().Cast<IFavorite>().ToList();
            }
        }

        /// <summary>
        /// For code generated by designer
        /// </summary>
        public Group(){ }

        internal Group(string name)
        {
            this.Name = name;
        }

        public void AddFavorite(IFavorite favorite)
        {
            AddFavoriteToDatabase(favorite);
            Data.Group.ReportGroupChanged(this);
        }

        private void AddFavoriteToDatabase(IFavorite favorite)
        {
            this.Favorites.Add((Favorite)favorite);
        }

        public void AddFavorites(List<IFavorite> favorites)
        {
            AddFavoritesToDatabase(favorites);
            Data.Group.ReportGroupChanged(this);
        }

        private void AddFavoritesToDatabase(List<IFavorite> favorites)
        {
            foreach (IFavorite favorite in favorites)
            {
                AddFavoriteToDatabase(favorite);
            }
        }

        public void RemoveFavorite(IFavorite favorite)
        {
            RemoveFavoriteFromDatabase(favorite);
            Data.Group.ReportGroupChanged(this);
        }

        public void RemoveFavorites(List<IFavorite> favorites)
        {
            RemoveFavoritesFromDatabase(favorites);
            Data.Group.ReportGroupChanged(this);
        }

        private void RemoveFavoritesFromDatabase(List<IFavorite> favorites)
        {
            foreach (IFavorite favorite in favorites)
            {
                RemoveFavoriteFromDatabase(favorite);
            }
        }

        private void RemoveFavoriteFromDatabase(IFavorite favorite)
        {
            var toRemove = favorite as Favorite;
            this.Favorites.Attach(toRemove);
            this.Favorites.Remove(toRemove);
        }

        bool IStoreIdEquals<IGroup>.StoreIdEquals(IGroup oponent)
        {
            var oponentGroup = oponent as Group;
            if (oponentGroup == null)
                return false;

            return oponentGroup.Id == this.Id;
        }

        public override string ToString()
        {
            return Data.Group.ToString(this, this.Id.ToString());
        }
    }
}
