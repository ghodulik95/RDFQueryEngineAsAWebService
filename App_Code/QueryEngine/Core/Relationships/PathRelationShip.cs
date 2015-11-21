// GBE - Graph By Example Query Interface
// Copyright (C) 2014-2015 Cheng Yang & Shi Qiao
// 
// This program is free software; you can redistribute it and/or modify it under 
// the terms of the GNU General Public License as published by the Free Software 
// Foundation; either version 3 of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT 
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS 
// FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with 
// this program; if not, write to the Free Software Foundation, Inc., 
// 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;

namespace GBE.Core
{
    public sealed class PathRelationShip : Relationship
    {
        Vertex vertex;
        IEntity entity;

        /// <exception cref="ArgumentNullException">
        /// <paramref name="vertex"/> is null.-or-
        /// <paramref name="entity"/> is null.
        /// </exception>
        internal PathRelationShip(Vertex vertex, IEntity entity)
        {
            if (vertex == null)
                throw new ArgumentNullException("vertex");
            if (entity == null)
                throw new ArgumentNullException("entity");

            this.vertex = vertex;
            this.entity = entity;
            Attach();
        }

        public override RelationshipType RelationshipType
        {
            get { return RelationshipType.Path; }
        }

        public override bool SupportsLabel
        {
            get { return true; } //make it support label.
        }

        public override IEntity First
        {
            get { return vertex; }
            protected set { vertex = (Vertex)value; }
        }

        //the following to attributes are meant to target the tow related shapes as vertex.
        public Vertex FirstVertex
        {
            get { return vertex; }
            set { vertex = (Vertex)value; }
        }

        public Vertex SecondVertex
        {
            get { return (Vertex)entity; }
            set { entity = value; }
        }

        public override IEntity Second
        {
            get { return entity; }
            protected set { entity = value; }
        }

        public PathRelationShip Clone(Vertex vertex, IEntity entity)
        {
            PathRelationShip relationship = new PathRelationShip(vertex, entity);
            relationship.CopyFrom(this);
            return relationship;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} --- {2}",
                "Vertex", vertex.ToString(), entity.Name);
        }
    }
}
