using System;
using Guineu.Expression;

namespace Guineu.Data.Dbf
{
	class IndexNavigator : INavigator
	{
		IndexTag tag;
		ExpressionBase filter;

		IndexNodeExterior node;
		Int32 keyIndex;

		Boolean indexStateValid = true;
		Int32 currentRecord;

		Boolean isBof;
		Boolean isEof;
		Boolean notInIndex;

		public IndexNavigator(IndexTag tag)
		{
			if (tag == null)
				throw new ArgumentNullException("tag");
			this.tag = tag;
		}

		public void GoTop()
		{
			indexStateValid = true;
			IndexNode curNode = tag.NodeReader.GetNode(tag.Header.RootNode, tag.Header.KeyLength);
			while (curNode is IndexNodeInterior)
			{
				Int32 nextNode = ((IndexNodeInterior)curNode).GetFirstNode();
				if (nextNode == 0)
					break;
				curNode = tag.NodeReader.GetNode(nextNode, tag.Header.KeyLength);
			}
			this.node = curNode as IndexNodeExterior;
			this.keyIndex = 0;
			if (this.node.GetKeyCount() == 0)
			{
				this.currentRecord = 1;
				isBof = true;
				isEof = true;
			}
			else
			{
				isBof = false;
				isEof = false;
				this.currentRecord = (Int32)this.node.GetRecordNumber(0);
			}
			while (!RecordIsValid())
				SkipForward();
		}
		public void GoBottom()
		{
			indexStateValid = true;
			IndexNode node = tag.NodeReader.GetNode(tag.Header.RootNode, tag.Header.KeyLength);
			while (node is IndexNodeInterior)
			{
				Int32 nextNode = ((IndexNodeInterior)node).GetLastNode();
				if (nextNode == 0)
					break;
				node = tag.NodeReader.GetNode(nextNode, tag.Header.KeyLength);
			}
			this.node = node as IndexNodeExterior;
			if (this.node.GetKeyCount() == 0)
			{
				this.keyIndex = 0;
				this.currentRecord = 1;
				isBof = true;
				isEof = true;
			}
			else
			{
				isBof = false;
				isEof = false;
				this.keyIndex = this.node.GetKeyCount() - 1;
				this.currentRecord = (Int32)this.node.GetRecordNumber(this.keyIndex);
			}
			while (!RecordIsValid())
				SkipBackward();
		}
		public void GoTo(long recNo)
		{
			isBof = false;
			isEof = false;
			currentRecord = (Int32)recNo;
			indexStateValid = false;
		}
		public void GoEof()
		{
			GoBottom();
			if (!isEof)
				SkipForward();
		}

		public void SkipForward()
		{
			isBof = false;
			isEof = false;
			ValidateIndexState();
			notInIndex = false;
			if (!IsEof)
				do
				{
					keyIndex = keyIndex + 1;
					if (this.keyIndex == this.node.GetKeyCount())
					{
						Int32 nextNode = node.GetNextNode();
						if (nextNode > 0)
						{
							node = (IndexNodeExterior)tag.NodeReader.GetNode(nextNode, tag.Header.KeyLength);
							keyIndex = 0;
						}
						else
						{
							isEof = true;
							break;
						}
					}
					currentRecord = (Int32) node.GetRecordNumber(keyIndex);
				} while (!RecordIsValid());
		}
		public void SkipBackward()
		{
			isBof = false;
			isEof = false;
			notInIndex = false;
			ValidateIndexState();
			if (!IsEof)
				do
				{
					keyIndex = keyIndex - 1;
					if (this.keyIndex < 0)
					{
						Int32 prevNode = node.GetPreviousNode();
						if (prevNode > 0)
						{
							node = (IndexNodeExterior)tag.NodeReader.GetNode(prevNode, tag.Header.KeyLength);
							keyIndex = this.node.GetKeyCount();
							break;
						}
					}
					currentRecord = (Int32) node.GetRecordNumber(keyIndex);
				} while (!RecordIsValid());
		}

		public Int64 RecordNumber
		{
			get
			{
				// TODO: A better approach might be to store the record number
				//       and locate it dynamically. This way we can more easily
				//       handle modified indexes and changed record values.
				if (indexStateValid)
					if (node == null)
						return 0;
					else
						if (isEof)
							return 0;
						else
							if (notInIndex)
								return currentRecord;
							else
								return node.GetRecordNumber(keyIndex);
				else
					return currentRecord;
			}
		}

		public Boolean IsEof
		{
			get
			{
				ValidateIndexState();
				if (notInIndex)
					return false;
				if (node == null)
					return true;
				if (isEof)
					return true;
				if (node.GetNextNode() < 0)
					if (keyIndex >= node.GetKeyCount())
						return true;
				return false;
			}
		}
		public Boolean IsBof
		{
			get
			{
				ValidateIndexState();
				if (notInIndex)
					return false;
				if (node == null)
					return true;
				if (isBof)
					return true;
				if (node.GetPreviousNode() < 0)
				{
					if (keyIndex < 0)
						return true;
					if (keyIndex == 0 && node.GetKeyCount() == 0)
						return true;
				}
				return false;
			}
		}

		public void SetFilter(Guineu.Expression.ExpressionBase filter)
		{
			this.filter = filter;
		}
		Boolean RecordIsValid()
		{
			if (filter == null)
				return true;
			else
				return filter.GetBool() || IsBof || IsEof;
		}

		/// <summary>
		/// Ensure that the current position in the index is valid.
		/// </summary>
		private void ValidateIndexState()
		{
			if (!indexStateValid)
			{
				Int32 startRecord = currentRecord;

				// TODO: For now we implement the slow version. The fast version evaluates
				//       the current key expression and uses this value plus the record number
				//       to perform a SEEK. And we would only validate the index state when
				//       we actually have to move.
				indexStateValid = true;
				GoTop();
				while (!IsEof && currentRecord != startRecord)
					SkipForward();

				// If we didn't find a record in the index, we still move back to the original one. 
				// This way record operation will work, but SKIP behaves as if it's at EOF.
				if (IsEof)
				{
					isBof = false;
					isEof = false;
					currentRecord = startRecord;
					notInIndex = true;
				}
				else
					notInIndex = false;
			}
		}
	}
}
