using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WackyBag.Structures.Collections
{
	public class RBTEx<TKey, TNode> : IEnumerable<TNode>
		where TNode : RBTEx<TKey, TNode>.NodeBase
		where TKey :notnull
	{
		public abstract class NodeBase
		{
			internal static bool IsNullOrBlack(TNode? node) => node == null || node.IsBlack;
			internal static bool IsNonNullRed(TNode? node) => node != null && node.IsRed;
			internal static bool IsNonNullBlack(TNode? node) => node != null && node.IsBlack;
			public override string ToString()
			{
				return $"{Key};{count},{changeCount},{isRed};{left?.Key.ToString() ?? "null"},{right?.Key.ToString() ?? "null"};{previous?.Key.ToString() ?? "null"},{next?.Key.ToString() ?? "null"}";
			}
			public abstract TKey Key { get; }
			private TNode? left;
			private TNode? right;
			internal TNode? previous;
			internal TNode? next;
			public TNode? Left { get => left; }
			public TNode? Right { get => right; }
			public TNode? Previous { get => previous; }
			public TNode? Next { get => next; }
			internal bool isRed;
			internal bool IsBlack
			{
				get => !isRed;
			}
			internal bool IsRed
			{
				get => isRed;
			}
			internal void ColorBlack() => isRed = false;
			internal void ColorRed() => isRed = true;
			internal void SetColor(bool isRed) => this.isRed = isRed;
			private int count = 1;
			private int changeCount = 0;
			private bool countUpdated = false;
			internal void SetUnupdated() => countUpdated = false;
			private int UpdateChange()
			{
				CheckCount();
				int change = changeCount;
				changeCount = 0;
				count += change;
				return change;
			}
			private int CheckCount()
			{
				if (!countUpdated)
				{
					left?.CheckCount();
					right?.CheckCount();
					changeCount += left?.UpdateChange() ?? 0;
					changeCount += right?.UpdateChange() ?? 0;
					countUpdated = true;
				}
				return count + changeCount;
			}
			public int Count { get => CheckCount(); }
			internal TNode? GetChild(bool atLeft)
			{
				if (atLeft) return Left;
				return Right;
			}
			internal TNode? GetSibling(TNode child)
			{
				if (child.IsAtLeft((TNode)this)) return Right;
				else return Left;
			}
			internal bool IsAtLeft(TNode Parent) => ReferenceEquals(Parent.Left, this);
			internal TNode? SetLeft(TNode? New)
			{
				changeCount += Left?.UpdateChange() ?? 0;
				changeCount -= Left?.Count ?? 0;
				TNode? node = Left;
				left = New;
				changeCount += Left?.UpdateChange() ?? 0;
				changeCount += Left?.Count ?? 0;
				countUpdated = false;
				return node;
			}
			internal TNode? SetRight(TNode? New)
			{
				changeCount += Right?.UpdateChange() ?? 0;
				changeCount -= Right?.Count ?? 0;
				TNode? node = Right;
				right = New;
				changeCount += Right?.UpdateChange() ?? 0;
				changeCount += Right?.Count ?? 0;
				countUpdated = false;
				return node;
			}
			internal TNode? ReplaceChild(bool atleft, TNode New)
			{
				if (atleft) return SetLeft(New);
				else return SetRight(New);
			}
			internal TNode ReplaceChild(TNode child, TNode New)
			{
				if (ReferenceEquals(Left, child)) return SetLeft(New)!;
				else return SetRight(New)!;
			}
			internal TNode? RotateLeft(ReplaceFn ReplaceFn)
			{
				//TNode node = right;
				//right = node.left;
				//node.left = this;
				//return node;
				TNode node = SetRight(null)!;
				SetRight(node.SetLeft(ReplaceFn((TNode)this, node)));
				return node;
			}
			internal TNode? RotateLeftRight(ReplaceFn ReplaceFn)
			{
				//TNode child = SetLeft(null);
				//TNode grandChild = child.SetRight(null);
				////left = grandChild.right;
				//SetLeft(grandChild.Right);
				////grandChild.right = this;
				//grandChild.SetRight(this);
				////child.right = grandChild.left;
				//child.SetRight(grandChild.Left);
				////grandChild.left = child;
				//grandChild.SetLeft(child);
				//child.SetRight(grandChild.SetLeft(SetLeft(grandChild.SetRight(ReplaceFn(this, grandChild)))));
				//return grandChild;
				Left!.RotateLeft(ReplaceChild);
				return RotateRight(ReplaceFn);

			}
			internal TNode? RotateRight(ReplaceFn ReplaceFn)
			{
				//TNode node = left;
				//left = node.right;
				//node.right = this;
				//return node;
				TNode node = SetLeft(null)!;
				SetLeft(node.SetRight(ReplaceFn((TNode)this, node)));
				return node;
			}
			internal TNode? RotateRightLeft(ReplaceFn ReplaceFn)
			{
				//TNode child = SetRight(null);
				//TNode grandChild = child.SetLeft(null);
				////right = grandChild.left;
				//SetRight(grandChild.Left);
				////grandChild.left = this;
				//grandChild.SetLeft(this);
				////child.left = grandChild.right;
				//child.SetLeft(grandChild.Right);
				////grandChild.right = child;
				//child.SetLeft(grandChild.SetRight(SetRight(grandChild.SetLeft(ReplaceFn(this, grandChild)))));
				//return grandChild;
				Right!.RotateRight(ReplaceChild);
				return RotateLeft(ReplaceFn);
			}
			internal bool Is2Node => IsBlack && IsNullOrBlack(Left) && IsNullOrBlack(Right);
			internal bool Is4Node => IsNonNullRed(Left) && IsNonNullRed(Right);
			internal void Split4Node()
			{
				ColorRed();
				Left!.ColorBlack();
				Right!.ColorBlack();
			}
			internal void Merge2Nodes()
			{
				ColorBlack();
				Left!.ColorRed();
				Right!.ColorRed();
			}
			internal TNode? Rotate(TNode current, TNode sibling, ReplaceFn func)
			{
				bool currentIsLeftChild = Left == current;
				TNode removeRed;
				if (IsNonNullRed(sibling.Left))
				{
					if (currentIsLeftChild)
					{
						return RotateRightLeft(func);
					}
					else
					{
						removeRed = Left!.Left!;
						removeRed.ColorBlack();
						return RotateRight(func);
					}
				}
				else
				{
					if (currentIsLeftChild)
					{
						removeRed = Right!.Right!;
						removeRed.ColorBlack();
						return RotateLeft(func);
					}
					else
					{
						return RotateLeftRight(func);
					}

				}
			}
			//public int ToTreeString(List<string> L, int l = 0, int p = 0)
			//{
			//	int p2 = p;
			//	if (L.Count <= l) L.Add("");
			//	string valueStr = "|" + (isRed ? "r" : "b") + (Key?.ToString() ?? "null");
			//	if (L[l].Length <= p) L[l] += Utils.Repeat(" ", p - L[l].Length + 1);
			//	L[l] = L[l].Insert(p, valueStr);
			//	p += valueStr.Length;
			//	if (Left != null)
			//	{
			//		p2 = Left.ToTreeString(L, l + 1, p2);
			//	}
			//	else
			//	{
			//		if (L.Count <= l) L.Add("|");
			//		p2 += 1;
			//	}
			//	if (Right != null)
			//	{
			//		p2 = Right.ToTreeString(L, l + 1, p2);
			//	}
			//	else
			//	{
			//		if (L.Count <= l) L.Add("|");
			//		p2 += 1;
			//	}
			//	return Utils.Max(p, p2);
			//}
		}

		internal delegate TNode? ReplaceFn(TNode from, TNode to);
		internal static bool IsNullOrBlack(TNode? node) => node == null || node.IsBlack;
		internal static bool IsNonNullRed(TNode? node) => node != null && node.IsRed;
		internal static bool IsNonNullBlack(TNode? node) => node != null && node.IsBlack;
		private TNode? root;
		private TNode? first;
		private TNode? last;
		private void Link(TNode? a, TNode? b)
		{
			if (a != null) a.next = b;
			else first = b;
			if (b != null) b.previous = a;
			else last = a;
		}
		private void Link(TNode? a, TNode? b, TNode? c)
		{
			Link(a, b); Link(b, c);
		}
		private void RemoveLink(TNode a)
		{
			Link(a.previous, a.next);
		}
		private TNode NewToTree(TNode New, TNode current, bool atleft)
		{
			if (atleft)
			{
				current.SetLeft(New);
				Link(current.previous, New, current);
			}
			else
			{
				current.SetRight(New);
				Link(current, New, current.next);
			}
			New.SetColor(true);
			return New;
		}
		public TNode? Root { get => root; }
		private readonly IComparer<TKey> comparer;
		public RBTEx(IComparer<TKey>? comparer = null)
		{
			this.comparer = comparer ?? Comparer<TKey>.Default;
		}
		public int Count => root?.Count ?? 0;
		public bool IsReadOnly => false;
		public TNode? Last => last;
		public TNode? First => first;
		public virtual TNode? DoAdd(TNode NewNode)
		{
			TKey key = NewNode.Key;
			if (root == null)
			{
				// The tree is empty and this is the first key.
				root = first = last = NewNode;
				root.ColorBlack();
				return root;
			}

			// Search for a node at bottom to insert the new node.
			// If we can guarantee the node we found is not a 4-node, it would be easy to do insertion.
			// We split 4-nodes along the search path.
			TNode? current = root;
			TNode? parent = null;
			TNode? grandParent = null;
			TNode? greatGrandParent = null;

			// Even if we don't actually add to the set, we may be altering its structure (by doing rotations and such).
			// So update `_version` to disable any instances of Enumerator/TreeSubSet from working on it.
			ClearStoreLast();
			int order = 0;
			while (current != null)
			{
				order = comparer.Compare(key, current.Key);
				if (order == 0)
				{
					// We could have changed root node to red during the search process.
					// We need to set it to black before we return.
					root.ColorBlack();
					return current;
				}

				// Split a 4-node into two 2-nodes.
				if (current.Is4Node)
				{
					current.Split4Node();
					// We could have introduced two consecutive red nodes after split. Fix that by rotation.
					if (IsNonNullRed(parent))
					{
						InsertionBalance(current, ref parent!, grandParent!, greatGrandParent!);
					}
				}
				current.SetUnupdated();

				greatGrandParent = grandParent;
				grandParent = parent;
				parent = current;
				//current = (order < 0) ? current.Left : current.Right;

				if (order < 0)
					current = current.Left;
				else
				{ lastIndex += current.Left?.Count ?? 0 + 1; current = current.Right; }

			}

			Debug.Assert(parent != null);
			// We're ready to insert the new node.
			NewToTree(NewNode, parent, order < 0);
			//if (order > 0)
			//{
			//	//parent.Right = node;
			//	parent.SetRight(node);
			//}
			//else
			//{
			//	//parent.Left = node;
			//	parent.SetLeft(node);
			//}
			// The new node will be red, so we will need to adjust colors if its parent is also red.
			if (parent.IsRed)
			{
				InsertionBalance(NewNode, ref parent!, grandParent!, greatGrandParent!);
			}

			// The root node is always black.
			root.ColorBlack();
			lastNode = NewNode;
			lastOrder = 0;
			return NewNode;
		}
		public virtual TNode? DoRemove(TKey key)
		{
			ClearStoreLast();
			if (root == null)
			{
				return null;
			}

			// Search for a node and then find its successor.
			// Then copy the key from the successor to the matching node, and delete the successor.
			// If a node doesn't have a successor, we can replace it with its left child (if not empty),
			// or delete the matching node.
			//
			// In top-down implementation, it is important to make sure the node to be deleted is not a 2-node.
			// Following code will make sure the node on the path is not a 2-node.

			// Even if we don't actually remove from the set, we may be altering its structure (by doing rotations
			// and such). So update our version to disable any enumerators/subsets working on it.

			TNode? current = root;
			TNode? parent = null;
			TNode? grandParent = null;
			TNode? match = null;
			TNode? parentOfMatch = null;
			bool foundMatch = false;
			while (current != null)
			{
				if (current.Is2Node)
				{
					// Fix up 2-node
					if (parent == null)
					{
						// `current` is the root. Mark it red.
						current.ColorRed();
					}
					else
					{
						TNode sibling = parent.GetSibling(current)!;
						if (sibling.IsRed)
						{
							// If parent is a 3-node, flip the orientation of the red link.
							// We can achieve this by a single rotation.
							// This case is converted to one of the other cases below.
							Debug.Assert(parent.IsBlack);
							var ReplaceFn = ReplaceChildOrRootFn(grandParent);
							if (parent.Right == sibling)
							{
								parent.RotateLeft(ReplaceFn);
							}
							else
							{
								parent.RotateRight(ReplaceFn);
							}

							parent.ColorRed();
							sibling.ColorBlack(); // The red parent can't have black children.
												  // `sibling` becomes the child of `grandParent` or `root` after rotation. Update the link from that node.
												  //ReplaceChildOrRoot(grandParent, parent, sibling);
												  // `sibling` will become the grandparent of `current`.
							grandParent = sibling;
							if (parent == match)
							{
								parentOfMatch = sibling;
							}

							sibling = parent.GetSibling(current)!;
						}

						Debug.Assert(IsNonNullBlack(sibling));

						if (sibling.Is2Node)
						{
							parent.Merge2Nodes();
						}
						else
						{
							// `current` is a 2-node and `sibling` is either a 3-node or a 4-node.
							// We can change the color of `current` to red by some rotation.
							TNode newGrandParent = parent.Rotate(current, sibling, ReplaceChildOrRootFn(grandParent))!;

							newGrandParent.SetColor(parent.IsRed);
							parent.ColorBlack();
							current.ColorRed();

							//ReplaceChildOrRoot(grandParent, parent, newGrandParent);
							if (parent == match)
							{
								parentOfMatch = newGrandParent;
							}
#pragma warning disable IDE0059 // 不需要赋值
							grandParent = newGrandParent;
#pragma warning restore IDE0059 // 不需要赋值
						}
					}
				}

				// We don't need to compare after we find the match.
				int order = foundMatch ? -1 : comparer.Compare(key, current.Key);
				if (order == 0)
				{
					// Save the matching node.
					foundMatch = true;
					match = current;
					parentOfMatch = parent;
				}

				grandParent = parent;
				parent = current;
				// If we found a match, continue the search in the right sub-tree.
				current.SetUnupdated();
				current = order < 0 ? current.Left : current.Right;
			}

			// Move successor to the matching node position and replace links.
			if (match != null)
			{
				ReplaceNode(match, parentOfMatch!, parent!, grandParent!);
				RemoveLink(match);
			}

			root?.ColorBlack();
			return match;
		}
		// After calling InsertionBalance, we need to make sure `current` and `parent` are up-to-date.
		// It doesn't matter if we keep `grandParent` and `greatGrandParent` up-to-date, because we won't
		// need to split again in the next node.
		// By the time we need to split again, everything will be correctly set.
		private void InsertionBalance(TNode current, ref TNode parent, TNode grandParent, TNode greatGrandParent)
		{
			Debug.Assert(parent != null);
			Debug.Assert(grandParent != null);

			bool parentIsOnRight = grandParent.Right == parent;
			bool currentIsOnRight = parent.Right == current;

			TNode newChildOfGreatGrandParent;
			var ReplaceFn = ReplaceChildOrRootFn(greatGrandParent);
			if (parentIsOnRight == currentIsOnRight)
			{
				// Same orientation, single rotation
				newChildOfGreatGrandParent = currentIsOnRight ? grandParent.RotateLeft(ReplaceFn)! : grandParent.RotateRight(ReplaceFn)!;
			}
			else
			{
				// Different orientation, double rotation
				newChildOfGreatGrandParent = currentIsOnRight ? grandParent.RotateLeftRight(ReplaceFn)! : grandParent.RotateRightLeft(ReplaceFn)!;
				// Current node now becomes the child of `greatGrandParent`
				parent = greatGrandParent;
			}

			// `grandParent` will become a child of either `parent` of `current`.
			grandParent.ColorRed();
			newChildOfGreatGrandParent.ColorBlack();

			//ReplaceChildOrRoot(greatGrandParent, grandParent, newChildOfGreatGrandParent);
		}
		/// <summary>
		/// Replaces the child of a parent node, or replaces the root if the parent is <c>null</c>.
		/// </summary>
		/// <param name="parent">The (possibly <c>null</c>) parent.</param>
		/// <param name="child">The child node to replace.</param>
		/// <param name="newChild">The node to replace <paramref name="child"/> with.</param>
		private TNode ReplaceChildOrRoot(TNode? parent, TNode child, TNode newChild)
		{
			if (parent != null)
			{
				return parent.ReplaceChild(child, newChild);
			}
			else
			{
				root = newChild;
				return child;
			}
		}
		private ReplaceFn ReplaceChildOrRootFn(TNode? parent) => (c, n) => ReplaceChildOrRoot(parent, c, n);
		/// <summary>
		/// Replaces the matching node with its successor.
		/// </summary>
		private void ReplaceNode(TNode match, TNode parentOfMatch, TNode successor, TNode parentOfSuccessor)
		{
			Debug.Assert(match != null);

			if (successor == match)
			{
				// This node has no successor. This can only happen if the right child of the match is null.
				Debug.Assert(match.Right == null);
				successor = match.Left!;
			}
			else
			{
				Debug.Assert(parentOfSuccessor != null);
				Debug.Assert(successor.Left == null);
				Debug.Assert((successor.Right == null && successor.IsRed) || (successor.Right!.IsRed && successor.IsBlack));

				successor.Right?.ColorBlack();

				if (parentOfSuccessor != match)
				{
					// Detach the successor from its parent and set its right child.

					//parentOfSuccessor.Left = successor.Right;
					//successor.Right = match.Right;
					parentOfSuccessor.SetLeft(successor.SetRight(match.SetRight(null)));
				}

				//successor.Left = match.Left;
				successor.SetLeft(match.SetLeft(null));
			}

			//successor.Color = match.Color;
			successor?.SetColor(match.IsRed);

			ReplaceChildOrRoot(parentOfMatch, match, successor!);
		}
		public virtual TNode? Find(TKey key)
		{
			if (lastNode?.Key.Equals(key) ?? false) return lastNode;
			var t = FindClosest(key);
			if (t.Item2 == 0) return t.Item1;
			else return null;
		}
		public (TNode, int) FindClosest(TKey key)
		{
			if ((lastNode != null) && (lastOrder == comparer.Compare(key, lastNode.Key)) && (
					lastOrder == 0 || (lastOrder < 0 && lastNode.Left == null) || (lastOrder > 0 && lastNode.Right == null)
				)) return (lastNode, lastOrder);
			ClearStoreLast();
			lastNode = root;
			while (lastNode != null)
			{
				lastOrder = comparer.Compare(key, lastNode.Key);
				if (lastOrder == 0) break;
				if (lastOrder < 0)
					if (lastNode.Left == null) break;
					else lastNode = lastNode.Left;
				else
					if (lastNode.Right == null) break;
				else { lastIndex += lastNode.Count + 1; lastNode = lastNode.Right; }
			}
			return (lastNode!, lastOrder);
		}
		public virtual TNode? FindByIndex(int index)
		{
			if (lastIndex == index) return lastNode;
			ClearStoreLast();
			if(root==null) return null;
			TNode? NextNode=root;lastNode= null;
			while (NextNode!=null) {
				if (NextNode.Count == index) return lastNode;
				else if (NextNode.Count < index)
				{
					lastNode = NextNode;
					NextNode = NextNode.Left;
				}
				else { 
					index-= NextNode.Count;
					lastNode = NextNode;
					NextNode = NextNode.Right;
				}
			}
			return lastNode;
			//lastNode = root;
			//while (lastNode != null)
			//{
			//	if (lastNode.Count == index) break;
			//	else if (lastNode.Count < index) lastNode = lastNode.Left;
			//	index -= lastNode.Left?.Count ?? 0 + 1;
			//	lastNode = lastNode.Right;
			//}
			//return lastNode;
		}
		public virtual int GetIndex(TKey key)
		{
			if (lastNode?.Key.Equals(key) ?? false) return lastIndex;
			ClearStoreLast();
			lastNode = root;
			while (lastNode != null)
			{
				int order = comparer.Compare(key, lastNode.Key);
				if (order == 0) return lastIndex;
				else if (order < 0) lastNode = lastNode.Left;
				else
				{
					lastIndex += lastNode.Left?.Count ?? 0 + 1;
					lastNode = lastNode.Right;
				}
			}
			return lastIndex;
		}
		public TNode? FindNotGreater(TKey key)
		{
			var V = FindClosest(key);
			if (V.Item2 > 0) return V.Item1.Previous;
			return V.Item1;
		}
		public TNode? FindGreater(TKey key)
		{
			var V = FindClosest(key);
			if (V.Item2 <= 0) return V.Item1.Next;
			return V.Item1;
		}
		//public override string ToString()
		//{
		//	List<string> res = new List<string>();
		//	root?.ToTreeString(res);
		//	return string.Join("\n", res);
		//}
		public virtual void ForEach(Action<TNode> action)
		{
			TNode? node = First;
			while (node != null)
			{
				action(node);
				node = node.next;
			}
		}
		public void Clear()
		{
			root = first = last = null;
		}
		public bool TryGetValue(TKey key, out TNode? node)
		{
			node = Find(key);
			return node != null;
		}
		public IEnumerable<TNode> Enumerate()
		{
			TNode? node = First;
			while (node != null)
			{
				yield return node;
				node = node.Next;
			}
		}
		public IEnumerator<TNode> GetEnumerator() => Enumerate().GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => Enumerate().GetEnumerator();

		protected TNode? lastNode;
		public TNode? LastNode => lastNode;
		protected int lastIndex;
		public int LastIndex => lastIndex;
		protected int lastOrder;
		public int LastOrder => lastOrder;
		public void ClearStoreLast()
		{
			lastNode = null;
			lastIndex = -1;
			lastOrder = int.MaxValue;
		}
	}

	public class SetEx<T> : RBTEx<T, SetEx<T>.Node>, ICollection<T>,IEnumerable<T>
		where T :notnull
	{

		public class Node : NodeBase
		{
			public override T Key { get; }
			public Node(T item)
			{
				Key = item;
			}
		}
		public void Add(T item) => DoAdd(item);
		public Node? DoAdd(T item) => DoAdd(new Node(item));
		public bool Contains(T item) => Find(item) != null;
		public void CopyTo(T[] array, int arrayIndex)
		{
			int i = arrayIndex;
			foreach (var j in this)
			{
				array[i++] = j;
			}
		}
		public bool Remove(T item) => DoRemove(item) != null;

		public new IEnumerator<T> GetEnumerator()
		{
			foreach (var i in Enumerate()) yield return i.Key;
		}
		public SetEx(IEnumerable<T>? values=null) {
			if (values != null)
			{
				foreach (var item in values)
				{
					Add(item);
				}
			}
		}
	}

	public class MapEx<TKey, TValue> : RBTEx<TKey, MapEx<TKey, TValue>.Node>, IDictionary<TKey, TValue>
		where TKey : notnull
	{

		public class Node : NodeBase
		{
			public override TKey Key { get; }
			public TValue Value;
			public KeyValuePair<TKey, TValue> Pair => new(Key, Value);
			public Node(TKey key, TValue value)
			{
				Key = key; Value = value;
			}
		}
		public TValue this[TKey key]
		{
			get
			{
				Node? node = Find(key);
				if (node != null) return node.Value;
				else return default!;
			}
			set
			{

				Node? node = Find(key);
				if (node != null) node.Value = value;
				else DoAdd(key, value);
			}
		}
		public ICollection<TKey> Keys
		{
			get
			{
				List<TKey> L = [];
				foreach (var i in this)
				{
					L.Add(i.Key);
				}
				return L;
			}
		}
		public ICollection<TValue> Values
		{
			get
			{
				List<TValue> L = [];
				foreach (var i in this)
				{
					L.Add(i.Value);
				}
				return L;
			}
		}
		public void Add(TKey key, TValue value) => DoAdd(key, value);
		public bool ContainsKey(TKey key)
		{
			return Find(key) != null;
		}
		public bool Remove(TKey key)
		{
			return DoRemove(key) != null;
		}
		public Node? DoAdd(TKey key, TValue value) => DoAdd(new Node(key, value));
		public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue SortValue)
		{
			Node? node = Find(key);
			if (node == null)
			{
				SortValue = default;
				return false;
			}
			SortValue = node.Value;
			return true;
		}

		public void Add(KeyValuePair<TKey, TValue> item) => DoAdd(item.Key, item.Value);

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			Node? node = Find(item.Key);
			if (node == null) return false;
			return Equals(node.Value, item.Value);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			int i = arrayIndex;
			foreach (var j in this)
			{
				array[i++] = j;
			}
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			Node? node = Find(item.Key);
			if (node == null) return false;
			Remove(item.Key);
			return true;
		}

		public new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			foreach (var i in Enumerate()) yield return i.Pair;
		}
		public MapEx(IEnumerable<KeyValuePair<TKey, TValue>>? values=null) {
            if (values!=null)
			{
				foreach (var item in values)
				{
					Add(item);
				}
			}
		}
	}
}
