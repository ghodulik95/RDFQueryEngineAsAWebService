using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GBE.Core
{
	
	public class Model : IProjectItem
	{
		string name;
		List<IEntity> entities = new List<IEntity>();
        int vertexID = 0;//used to record the vertex created.
		List<Relationship> relationships = new List<Relationship>();
        List<EdgeID> edgeLabel = new List<EdgeID>();
        List<EdgeID> pathLabel = new List<EdgeID>();
        Dictionary<int, string> vertexLabel = new Dictionary<int, string>();
		Project project = null;
		bool isDirty = false;
		bool loading = false;
        String description = "";
        //the following four variables are added for the component query for the query engine.
        int compCount = 1;//count of component;
        Dictionary<int, List<EdgeID>> compEdgeList = new Dictionary<int, List<EdgeID>>();
        Dictionary<int, List<VertexID>> compVertexList = new Dictionary<int, List<VertexID>>();
        List<CompPathID> compPathList = new List<CompPathID>();
        


		public event EventHandler Modified;
		public event EventHandler Renamed;
		public event EventHandler Closing;
		public event EntityEventHandler EntityAdded;
		public event EntityEventHandler EntityRemoved;
		public event RelationshipEventHandler RelationAdded;
		public event RelationshipEventHandler RelationRemoved;
		public event SerializeEventHandler Serializing;
		public event SerializeEventHandler Deserializing;

		public Model()
		{
            name = "Untitled";
		}
	
		/// <exception cref="ArgumentException">
		/// <paramref name="name"/> cannot be empty string.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="language"/> is null.
		/// </exception>
		public Model(string name)
		{
			if (name != null && name.Length == 0)
				throw new ArgumentException("Name cannot empty string.");

			this.name = name;
		}

		public string Name
		{
			get
			{
				if (name == null)
                    return "Untitled";
				else
					return name;
			}
			set
			{
				if (name != value && value != null)
				{
					name = value;
					OnRenamed(EventArgs.Empty);
					OnModified(EventArgs.Empty);
				}
			}
		}

        public String Description
        {
            get { return description; }
            set
            {
                if (description!=value && value != null)
                {
                    description = value;
                    OnModified(EventArgs.Empty);
                }
            }
        }

		public Project Project
		{
			get { return project; }
			set { project = value; }
		}

		public bool IsUntitled
		{
			get
			{
				return (name == null);
			}
		}

		public bool IsDirty
		{
			get { return isDirty; }
		}

		protected bool Loading
		{
			get { return loading; }
		}

		public bool IsEmpty
		{
			get
			{
				return (entities.Count == 0 && relationships.Count == 0);
			}
		}

		void IModifiable.Clean()
		{
			isDirty = false;
		}

		void IProjectItem.Close()
		{
			OnClosing(EventArgs.Empty);
		}

        //get the two lists.
        public List<EdgeID> EdgeLabel
        {
            get { return edgeLabel; }
        }

        public Dictionary<int, List<EdgeID>> CompEdgeList
        {
            get { return compEdgeList; }
        }

        public Dictionary<int, List<VertexID>> CompVertexList
        {
            get { return compVertexList; }
        }

        public List<CompPathID> CompPathList
        {
            get { return compPathList; }
        }

        public List<EdgeID> PathLabel
        {
            get { return pathLabel; }
        }

        public Dictionary<int,string> VertexLabel
        {
            get { return vertexLabel; }
        }



		public IEnumerable<IEntity> Entities
		{
			get { return entities; }
		}

		public IEnumerable<Relationship> Relationships
		{
			get { return relationships; }
		}

        public void reachComponent()
        {//generate component groups for edges and vertices.
            List<EdgeID> edgeList =edgeLabel;//the list of edges.
            Dictionary<int, String> vertexList = vertexLabel;
            //will show the components it belong to.
            //First build ajacency list, then for each vertex in the vertex list, use the ajacency list to do search until no one can go out.
            //vertexList starts from 0(as we have done refreshList).
            List<List<int>> aList = new List<List<int>>();
            foreach (int key in vertexList.Keys)
            {
                List<int> aListkey = new List<int>();
                aList.Add(aListkey);
            }
            foreach (EdgeID el in edgeList)//build the ajacency list.
            {
                int id1 = el.ID1;
                int id2 = el.ID1;
                aList[id1].Add(id2);
                aList[id2].Add(id1);
            }
            List<List<int>> components = new List<List<int>>();
            bool[] visited = new bool[aList.Count];//number of vertices, record if they are used or not.

            for (int i = 0; i < visited.Length; i++)
            {
                if (visited[i]) continue;

                List<int> component = new List<int>();
                component.Add(i);
                //travesal of graph using graph search.
                Stack idStack = new Stack();
                idStack.Push(i);
                while (idStack.Count != 0)
                {//currently possible duplicate of work because there are cycles.
                    int currrentID = (int)idStack.Pop();
                    if (visited[currrentID]) continue;
                    visited[currrentID] = true;
                    foreach (int nextID in aList[currrentID])
                    {
                        if (!component.Contains(nextID))//now it's OK;
                        {
                            component.Add(nextID);
                            idStack.Push(nextID);
                        }

                    }
                }
                components.Add(component);
            }
            compCount = components.Count;
            //vertexList and edgeList;
            //need to clear the dictionaries before adding new entries.
            compEdgeList = new Dictionary<int, List<EdgeID>>();
            compVertexList = new Dictionary<int, List<VertexID>>();
            for (int i = 0; i < compCount; i++)
            {
                List<int> sepCompVer = components[i];
                List<VertexID> sepVerList = new List<VertexID>();
                List<EdgeID> sepEdgeList = new List<EdgeID>();
                for (int j = 0; j < sepCompVer.Count; j++)
                {
                    VertexID vIDpair = new VertexID(sepCompVer[j], vertexLabel[sepCompVer[j]]);
                    sepVerList.Add(vIDpair);
                    foreach(EdgeID edgeIDpair in edgeLabel){
                        if (edgeIDpair.ID1==vIDpair.ID||edgeIDpair.ID2==vIDpair.ID && !sepEdgeList.Contains(edgeIDpair))
                        {
                            sepEdgeList.Add(edgeIDpair);//two nodes of an edge must be in the same component.
                        }
                    }
                }
                compEdgeList.Add(i,sepEdgeList);
                compVertexList.Add(i,sepVerList);
            }

            //pathlist
            foreach (EdgeID pathIDpair in pathLabel)
            {
                int compID1 = 0;
                int compID2 = 1;
                int nodeID1=pathIDpair.ID1;
                int nodeID2=pathIDpair.ID2;
                for (int i = 0; i < compCount; i++)
                {
                    if (components[i].Contains(nodeID1)) compID1 = i;
                    if (components[i].Contains(nodeID2)) compID2 = i;//two nodes of a path might be in the same component.
                }
                compPathList.Add(new CompPathID(compID1, nodeID1, compID2, nodeID2, pathIDpair.LabelOfEdge));
            }

        }

        public void RefreshVertexLabel()//generate the vertexIDLabelPair or refresh it.
        {
            int index = 0;//realocate the vertex ID starting from 0;
            foreach(IEntity entity in entities)
            {
                if(entity.EntityType==EntityType.Vertex)
                {
                    PairIDLable((Vertex)entity,index);
                    index++;
                }
            }

            //It will check if there're useless keys exist or not and then delete them.
            while(index<vertexID){
                if (vertexLabel.ContainsKey(index))
                {
                    vertexLabel.Remove(index);
                }
                index++;
            }
            
        }



        public void RefreshEdgeIDLabel()//this includes the refreshment of pathID label and edgeID label.
        {
            foreach (Relationship relation in relationships)
            {
                if (relation.RelationshipType == RelationshipType.Edge)
                {
                    UpdateEdgeID((EdgeRelationship)relation);
                }
                if (relation.RelationshipType == RelationshipType.Path)
                {
                    UpdatePathID((PathRelationShip)relation);
                }
            }
        }

        private void UpdateEdgeID(EdgeRelationship edgeRelationship)
        {
            foreach (EdgeID pair in edgeLabel)
            {
                if (pair.ID1 == edgeRelationship.FirstVertex.OldID && pair.ID2 == edgeRelationship.SecondVertex.OldID)// this is dangerous which requires updatae edgeID after refreshing vertex label.Will change later.
                {
                    pair.ID1 = edgeRelationship.FirstVertex.ID;
                    pair.ID2 = edgeRelationship.SecondVertex.ID;
                    pair.LabelOfEdge = edgeRelationship.Label;
                }
            }
        }

        private void UpdatePathID(PathRelationShip path)
        {
            foreach (EdgeID pair in pathLabel)
            {
                if (pair.ID1 == path.FirstVertex.OldID && pair.ID2 == path.SecondVertex.OldID)// this is dangerous which requires updatae edgeID after refreshing vertex label.Will change later.
                {
                    pair.ID1 = path.FirstVertex.ID;
                    pair.ID2 = path.SecondVertex.ID;
                    pair.LabelOfEdge = path.Label;
                }
            }
        }

        private void PairIDLable(Vertex vertex, int index)
        {//record old ID for the use of EdgeIDLabel;
            vertex.OldID = vertex.ID;
            vertex.ID = index;
            String vertexText = vertex.Text;
            if (vertexText.StartsWith("ub:"))
            {
                String newText = "<http://www.lehigh.edu/~zhp2/2004/0401/univ-bench.owl#" + vertexText.Substring(3) + ">";
                vertexText = newText;
            }
            if (vertexLabel.ContainsKey(index))
            {
                vertexLabel[index] = vertexText;
            }
            else
            {
                vertexLabel.Add(index, vertexText);
            }
            
        }

		private void ElementChanged(object sender, EventArgs e)
		{
			OnModified(e);
		}

		private void AddEntity(IEntity entity)
		{
			entities.Add(entity);
			entity.Modified += new EventHandler(ElementChanged);
			OnEntityAdded(new EntityEventArgs(entity));
		}

        public Vertex AddVertex()
		{
			Vertex vertex = new Vertex();
			AddVertex(vertex);
			return vertex;
		}

		protected virtual void AddVertex(Vertex vertex)
		{
            vertex.ID = vertexID++;//assign the ID to the vertex;
            //would not add at this time. Only add at the end.
            //vertexLabel.Add(vertex.ID, vertex.Text);
			AddEntity(vertex);
		}

		public bool InsertVertex(Vertex vertex)
		{
			if (vertex != null && !entities.Contains(vertex))
			{
				AddVertex(vertex);
				return true;
			}
			else
			{
				return false;
			}
		}

		private void AddRelationship(Relationship relationship)
		{
			relationships.Add(relationship);
			relationship.Modified += new EventHandler(ElementChanged);
			OnRelationAdded(new RelationshipEventArgs(relationship));
		}

		/// <exception cref="ArgumentNullException">
		/// <paramref name="vertex"/> or <paramref name="entity"/> is null.
		/// </exception>
		public virtual EdgeRelationship AddEdgeRelationship(Vertex vertex, IEntity entity)
		{
            EdgeRelationship edgeRelationship = new EdgeRelationship(vertex, entity);

            AddEdgeRelationship(edgeRelationship);
            return edgeRelationship;
		}

        protected virtual void AddEdgeRelationship(EdgeRelationship edgeRelationship)
		{
            AddRelationship(edgeRelationship);
            //the following two is to add the edges to the array we want. it is put here because both the insert new and the load will go through this stage.
            EdgeID edgeID = new EdgeID(edgeRelationship.FirstVertex.ID, edgeRelationship.SecondVertex.ID, edgeRelationship.Label);
            edgeLabel.Add(edgeID);

            
		}

        public bool InsertEdgeRelationship(EdgeRelationship edgeRelationship)
		{
            if (edgeRelationship != null && !relationships.Contains(edgeRelationship) &&
                entities.Contains(edgeRelationship.First) && entities.Contains(edgeRelationship.Second))
			{
                AddEdgeRelationship(edgeRelationship);

				return true;

			}
			else
			{
				return false;
			}
		}

        /// <exception cref="ArgumentNullException">
        /// <paramref name="vertex"/> or <paramref name="entity"/> is null.
        /// </exception>
        public virtual PathRelationShip AddPathRelationShip(Vertex vertex, IEntity entity)
        {
            PathRelationShip path = new PathRelationShip(vertex, entity);

            AddPathRelationShip(path);
            return path;
        }

        protected virtual void AddPathRelationShip(PathRelationShip path)
        {
            AddRelationship(path);
            //the following two is to add the edges to the array we want. it is put here because both the insert new and the load will go through this stage.
            EdgeID edgeID = new EdgeID(path.FirstVertex.ID, path.SecondVertex.ID, path.Label);
            pathLabel.Add(edgeID);
        }

        public bool InsertPathRelationShip(PathRelationShip path)
        {
            if (path != null && !relationships.Contains(path) &&
                entities.Contains(path.First) && entities.Contains(path.Second))
            {
                AddPathRelationShip(path);

                return true;

            }
            else
            {
                return false;
            }
        }

		public void RemoveEntity(IEntity entity)
		{
			if (entities.Remove(entity))
			{
				entity.Modified -= new EventHandler(ElementChanged);
				RemoveRelationships(entity);
				OnEntityRemoved(new EntityEventArgs(entity));
                if(entity.EntityType==EntityType.Vertex)
                {
                    //remove the corresponding label in the vertexLabel list.
                    RemoveVertexLabel((Vertex)entity);
                }
			}
		}

        //remove the last entity. For the display result reason.
        public void RemoveLastEntity()
        {
            RemoveEntity(entities.Last());
        }

        //add the function to it to remove from edgeID.
		private void RemoveRelationships(IEntity entity)
		{
			for (int i = 0; i < relationships.Count; i++)
			{
				Relationship relationship = relationships[i];
				if (relationship.First == entity || relationship.Second == entity)
				{
                    if (relationship.RelationshipType == RelationshipType.Edge)//if it's the vertex type.
                    {
                        //remove the corresponding edgeID related to the entity.
                        RemoveEdgeID((EdgeRelationship)relationship );
                    }
                    if (relationship.RelationshipType == RelationshipType.Path)
                    {
                        RemoveEdgeID((PathRelationShip)relationship);
                    }
					relationship.Detach();
					relationship.Modified -= new EventHandler(ElementChanged);
					relationships.RemoveAt(i--);
					OnRelationRemoved(new RelationshipEventArgs(relationship));
				}
			}
		}

        private void RemoveVertexLabel(Vertex vertex)
        {
            if(vertexLabel.ContainsKey(vertex.ID))
            {
                vertexLabel.Remove(vertex.ID);
            }
        }

        private void RemoveEdgeID(EdgeRelationship edgeRelationship)
        {
            //currently labels are empty.
            foreach(var item in edgeLabel)//don't need label to remove the edgeLabe.
            {
                if (item.ID1 == edgeRelationship.FirstVertex.ID && item.ID2 == edgeRelationship.SecondVertex.ID)
                {
                    edgeLabel.Remove(item);
                    break;
                }
            }
        }

        private void RemoveEdgeID(PathRelationShip path)
        {
            foreach (var item in pathLabel)//don't need label to remove the edgeLabe.
            {
                if (item.ID1 == path.FirstVertex.ID && item.ID2 == path.SecondVertex.ID)
                {
                    pathLabel.Remove(item);
                    break;
                }
            }
        }


		public void RemoveRelationship(Relationship relationship)
		{
			if (relationships.Contains(relationship))
			{

                if (relationship.RelationshipType == RelationshipType.Edge)
                {
                    //remove the corresponding edgeID list element.
                    RemoveEdgeID((EdgeRelationship)relationship);
                }
                if (relationship.RelationshipType == RelationshipType.Path)
                {
                    RemoveEdgeID((PathRelationShip)relationship);
                }
				relationship.Detach();
				relationship.Modified -= new EventHandler(ElementChanged);
				relationships.Remove(relationship);
				OnRelationRemoved(new RelationshipEventArgs(relationship));
			}
		}

		void IProjectItem.Serialize(XmlElement node)
		{
			Serialize(node);
		}

		void IProjectItem.Deserialize(XmlElement node)
		{
			Deserialize(node);
		}

        //build for clone, for the results display.
        //This is a deep copy way for the use of results set.
        //This use the xml serialization and deserialization.
        public Model Clone()
        {
            XmlDocument document = new XmlDocument();

            XmlElement node = document.CreateElement("Model");

            this.Serialize(node);

            Type type = this.GetType();
            XmlAttribute typeAttribute = node.OwnerDocument.CreateAttribute("type");
            typeAttribute.InnerText = type.FullName;
            node.Attributes.Append(typeAttribute);

            XmlAttribute assemblyAttribute = node.OwnerDocument.CreateAttribute("assembly");
            assemblyAttribute.InnerText = type.Assembly.FullName;
            node.Attributes.Append(assemblyAttribute);

            string typeName = typeAttribute.InnerText;
            string assemblyName = assemblyAttribute.InnerText;

            try
            {
                Assembly assembly = Assembly.Load(assemblyName);

                IProjectItem projectItem = (IProjectItem)assembly.CreateInstance(
                    typeName, false,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null, null, null, null);
                projectItem.Deserialize(node);

                projectItem.Clean();
                return (Model)projectItem;
            }
            catch (InvalidDataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("Invalid type or assembly of ProjectItem.", ex);
            }
        }


		/// <exception cref="ArgumentNullException">
		/// <paramref name="node"/> is null.
		/// </exception>
		private void Serialize(XmlElement node)
		{
			if (node == null)
				throw new ArgumentNullException("root");

			XmlElement nameElement = node.OwnerDocument.CreateElement("Name");
			nameElement.InnerText = Name;
			node.AppendChild(nameElement);

            XmlElement descriptionElement = node.OwnerDocument.CreateElement("Description");//to store the description
            descriptionElement.InnerText = Description;
            node.AppendChild(descriptionElement);

			SaveEntitites(node);
			SaveRelationships(node);

			OnSerializing(new SerializeEventArgs(node));
		}

		/// <exception cref="InvalidDataException">
		/// The save format is corrupt and could not be loaded.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="node"/> is null.
		/// </exception>
		private void Deserialize(XmlElement node)
		{
			if (node == null)
				throw new ArgumentNullException("root");
			loading = true;

			XmlElement nameElement = node["Name"];
			if (nameElement == null || nameElement.InnerText == "")
				name = null;
			else
				name = nameElement.InnerText;


            XmlElement descriptionElement = node["Description"];//load description.
            if (descriptionElement == null || descriptionElement.InnerText == "")
                Description = "";
            else
                Description = descriptionElement.InnerText;

			LoadEntitites(node);
			LoadRelationships(node);

			OnDeserializing(new SerializeEventArgs(node));
			loading = false;
		}

		/// <exception cref="InvalidDataException">
		/// The save format is corrupt and could not be loaded.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="root"/> is null.
		/// </exception>
		private void LoadEntitites(XmlNode root)
		{
			if (root == null)
				throw new ArgumentNullException("root");

			XmlNodeList nodeList = root.SelectNodes("Entities/Entity");

			foreach (XmlElement node in nodeList)
			{
				try
				{
					string type = node.GetAttribute("type");

					IEntity entity = GetEntity(type);
					entity.Deserialize(node);
				}
				catch (BadSyntaxException ex)
				{
					throw new InvalidDataException("Invalid entity.", ex);
				}
			}
		}

		private IEntity GetEntity(string type)
		{
			switch (type)
			{
                case "Vertex":
					return AddVertex();

				default:
					throw new InvalidDataException("Invalid entity type: " + type);
			}
		}

		/// <exception cref="InvalidDataException">
		/// The save format is corrupt and could not be loaded.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="root"/> is null.
		/// </exception>
		private void LoadRelationships(XmlNode root)
		{
			if (root == null)
				throw new ArgumentNullException("root");

			XmlNodeList nodeList = root.SelectNodes(
				"Relationships/Relationship"); 

			foreach (XmlElement node in nodeList)
			{
				string type = node.GetAttribute("type");
				string firstString = node.GetAttribute("first");
				string secondString = node.GetAttribute("second");
				int firstIndex, secondIndex;

				if (!int.TryParse(firstString, out firstIndex) ||
					!int.TryParse(secondString, out secondIndex))
				{
					throw new InvalidDataException("The save file is corrupt and could not be loaded.");
				}
				if (firstIndex < 0 || firstIndex >= entities.Count ||
					secondIndex < 0 || secondIndex >= entities.Count)
				{
					throw new InvalidDataException("Corrupt save format.");
				}

				try
				{
					IEntity first = entities[firstIndex];
					IEntity second = entities[secondIndex];
					Relationship relationship;

					switch (type)
					{
                        case "Path":
                        case "PathRelationShip":
                            relationship = AddPathRelationShip(first as Vertex, second);
                            break;
                        case "Edge":
						case "EdgeRelationship":
							relationship = AddEdgeRelationship(first as Vertex, second);
							break;

						default:
							throw new InvalidDataException(
								"Corrupt save format.");
					}
					relationship.Deserialize(node);
				}
				catch (ArgumentNullException ex)
				{
					throw new InvalidDataException("Invalid relationship.", ex);
				}
				catch (RelationshipException ex)
				{
					throw new InvalidDataException("Invalid relationship.", ex);
				}
			}
		}

		/// <exception cref="ArgumentNullException">
		/// <paramref name="node"/> is null.
		/// </exception>
		private void SaveEntitites(XmlElement node)
		{
			if (node == null)
				throw new ArgumentNullException("root");

			XmlElement entitiesChild = node.OwnerDocument.CreateElement("Entities");

			foreach (IEntity entity in entities)
			{
				XmlElement child = node.OwnerDocument.CreateElement("Entity");

				entity.Serialize(child);
				child.SetAttribute("type", entity.EntityType.ToString());
				entitiesChild.AppendChild(child);
			}
			node.AppendChild(entitiesChild);
		}

		/// <exception cref="ArgumentNullException">
		/// <paramref name="root"/> is null.
		/// </exception>
		private void SaveRelationships(XmlNode root)
		{
			if (root == null)
				throw new ArgumentNullException("root");

			XmlElement relationsChild = root.OwnerDocument.CreateElement("Relationships");

			foreach (Relationship relationship in relationships)
			{
				XmlElement child = root.OwnerDocument.CreateElement("Relationship");

				int firstIndex = entities.IndexOf(relationship.First);
				int secondIndex = entities.IndexOf(relationship.Second);

				relationship.Serialize(child);
				child.SetAttribute("type", relationship.RelationshipType.ToString());
				child.SetAttribute("first", firstIndex.ToString());
				child.SetAttribute("second", secondIndex.ToString());
				relationsChild.AppendChild(child);
			}
			root.AppendChild(relationsChild);
		}

		protected virtual void OnEntityAdded(EntityEventArgs e)
		{
			if (EntityAdded != null)
				EntityAdded(this, e);
			OnModified(EventArgs.Empty);
		}

		protected virtual void OnEntityRemoved(EntityEventArgs e)
		{
			if (EntityRemoved != null)
				EntityRemoved(this, e);
			OnModified(EventArgs.Empty);
		}

		protected virtual void OnRelationAdded(RelationshipEventArgs e)
		{
			if (RelationAdded != null)
				RelationAdded(this, e);
			OnModified(EventArgs.Empty);
		}

		protected virtual void OnRelationRemoved(RelationshipEventArgs e)
		{
			if (RelationRemoved != null)
				RelationRemoved(this, e);
			OnModified(EventArgs.Empty);
		}

		protected virtual void OnSerializing(SerializeEventArgs e)
		{
			if (Serializing != null)
				Serializing(this, e);
		}

		protected virtual void OnDeserializing(SerializeEventArgs e)
		{
			if (Deserializing != null)
				Deserializing(this, e);
			OnModified(EventArgs.Empty);
		}

		protected virtual void OnModified(EventArgs e)
		{
			isDirty = true;
			if (Modified != null)
				Modified(this, e);
		}

		protected virtual void OnRenamed(EventArgs e)
		{
			if (Renamed != null)
				Renamed(this, e);
		}

		protected virtual void OnClosing(EventArgs e)
		{
			if (Closing != null)
				Closing(this, e);
		}

		public override string ToString()
		{
			if (IsDirty)
				return Name + "*";
			else
				return Name;
		}
	}
}
