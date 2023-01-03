/*
 * DynamicScrollView.cs
 * 
 * @author mosframe / https://github.com/mosframe
 * 
 */



namespace Mosframe
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;
    using XLua;
    [CSharpCallLua]
    public delegate void ListItemRenderer(int index, IDynamicScrollViewItem item);

    /// <summary>
    /// Dynamic Scroll View
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public abstract class DynamicScrollView : UIBehaviour
    {
        private bool seedDataInit = false;
        private int _totalItemCount = 0;
        [HideInInspector]
        public int totalItemCount
        {
            get { return _totalItemCount; }
            set
            {
                // if (value != _totalItemCount)
                // {
                _totalItemCount = value;
                this.prevTotalItemCount = this.totalItemCount;

                // check scroll bottom

                // var isBottom = false;
                // if (this.viewportSize - this.contentAnchoredPosition >= this.contentSize - this.itemSize * 0.5f)
                // {
                //     isBottom = true;
                // }

                this.resizeContent();

                // move scroll to bottom

                // if (isBottom)
                // {
                //     this.contentAnchoredPosition = this.viewportSize - this.contentSize;
                // }
                this.refresh();
                //  }
            }
        }
        public RectTransform itemPrototype = null;

        public ListItemRenderer itemRenderer;
        public ListItemRenderer itemUpdateInfo; //刷新组件内的数据
        public ListItemRenderer activeFalse; //刷新组件内的数据
        public ListItemRenderer clearItemRender;
        public void ScrollToTop()
        {
            this.contentAnchoredPosition = 0; //顶端
            this.refresh();
        }
        public void scrollToLastPos()
        {

            this.contentAnchoredPosition = this.viewportSize - this.contentSize;
            this.refresh();
        }
        public void scrollByItemIndex(int itemIndex)
        {
            if (this.totalItemCount == 0) return;
            var totalLen = this.contentSize;
            var itemLen = totalLen / this.totalItemCount;
            var pos = itemLen * itemIndex;
            this.contentAnchoredPosition = -pos;
        }
        public void scrollByItemIndexToMiddle(int itemIndex)
        {
            if (this.totalItemCount == 0) return;
            var totalLen = this.contentSize;
            var itemLen = totalLen / this.totalItemCount;
            var pos = itemLen * (itemIndex - 3);
            this.contentAnchoredPosition = -pos;
        }
        public void refresh()
        {

            var index = 0;
            if (this.contentAnchoredPosition != 0)
            {
                index = (int)(-this.contentAnchoredPosition / this.itemSize);
            }

            foreach (var itemRect in this.containers)
            {
                // set item position
                var pos = this.itemSize * index;
                itemRect.anchoredPosition = (this.direction == Direction.Vertical) ? new Vector2(0, -pos) : new Vector2(pos, 0);

                this.updateItem(index, itemRect.gameObject);

                ++index;
            }

            this.nextInsertItemNo = index - this.containers.Count;
            this.prevAnchoredPosition = (int)(this.contentAnchoredPosition / this.itemSize) * this.itemSize;
        }

        public void updateListItemInfo()
        {
            // var index = 0;
            // if (this.contentAnchoredPosition != 0)
            // {
            //     index = (int)(-this.contentAnchoredPosition / this.itemSize);
            // }
            foreach (var itemRect in this.containers)
            {
                var item = itemRect.gameObject.GetComponent<IDynamicScrollViewItem>();
                if (item != null)
                {
                    if (itemUpdateInfo != null)
                    {
                        itemUpdateInfo(-1, item);
                    }
                }
                //  ++index;
            }
        }

        public void clearData()
        {
            foreach (var itemRect in this.containers)
            {
                var item = itemRect.gameObject.GetComponent<IDynamicScrollViewItem>();
                if (item != null)
                {
                    if (clearItemRender != null)
                    {
                        clearItemRender(-1, item);
                    }
                }
            }
        }

        protected override void Awake()
        {

            if (itemPrototype == null)
            {
                return;
            }

            base.Awake();

            scrollRect = GetComponent<ScrollRect>();
            viewportRect = scrollRect.viewport;
            contentRect = scrollRect.content;
        }
        protected override void Start()
        {

            this.prevTotalItemCount = this.totalItemCount;

            //防止它自己失活自动停掉所有携程
            FGUI.inst.StartCoroutine(onSeedData());
            //onSeedData();

        }


        //初始创建时，逐帧创建刷新小item
        protected IEnumerator onSeedData()
        {

            // hide prototype

            seedDataInit = false;

            this.itemPrototype.gameObject.SetActiveFalse();

            yield return null;

            // instantiate items

            var itemCount = (int)(this.viewportSize / this.itemSize) + 3;

            for (var i = 0; i < itemCount; ++i)
            {
                if (itemPrototype == null || contentRect == null) break;

                var itemRect = Instantiate(this.itemPrototype);
                itemRect.SetParent(this.contentRect, false);
                itemRect.name = i.ToString();
                itemRect.anchoredPosition = this.direction == Direction.Vertical ? new Vector2(0, -this.itemSize * i) : new Vector2(this.itemSize * i, 0);
                this.containers.AddLast(itemRect);

                itemRect.gameObject.SetActive(true);

                this.updateItem(i, itemRect.gameObject);
                yield return null;
            }

            // resize content

            if (this != null)
            {
                this.resizeContent();
                seedDataInit = true;
            }
           
        }


        private void Update()
        {

            if (!seedDataInit) return;

            if (this.totalItemCount != this.prevTotalItemCount)
            {

                this.prevTotalItemCount = this.totalItemCount;

                // check scroll bottom

                var isBottom = false;
                if (this.viewportSize - this.contentAnchoredPosition >= this.contentSize - this.itemSize * 0.5f)
                {
                    isBottom = true;
                }

                this.resizeContent();

                // move scroll to bottom

                if (isBottom)
                {
                    this.contentAnchoredPosition = this.viewportSize - this.contentSize;
                }

                this.refresh();
            }


            // [ Scroll up ]

            while (this.contentAnchoredPosition - this.prevAnchoredPosition < -this.itemSize * 2)
            {

                this.prevAnchoredPosition -= this.itemSize;

                // move a first item to last

                var first = this.containers.First;
                if (first == null) break;
                var item = first.Value;
                this.containers.RemoveFirst();
                this.containers.AddLast(item);

                // set item position

                var pos = this.itemSize * (this.containers.Count + this.nextInsertItemNo);
                item.anchoredPosition = (this.direction == Direction.Vertical) ? new Vector2(0, -pos) : new Vector2(pos, 0);

                // update item
                this.updateItem(this.containers.Count + this.nextInsertItemNo, item.gameObject);

                this.nextInsertItemNo++;
            }

            // [ Scroll down ]

            while (this.contentAnchoredPosition - this.prevAnchoredPosition > 0)
            {

                this.prevAnchoredPosition += this.itemSize;

                // move a last item to first

                var last = this.containers.Last;
                if (last == null) break;
                var item = last.Value;
                this.containers.RemoveLast();
                this.containers.AddFirst(item);

                this.nextInsertItemNo--;

                // set item position

                var pos = this.itemSize * this.nextInsertItemNo;
                item.anchoredPosition = (this.direction == Direction.Vertical) ? new Vector2(0, -pos) : new Vector2(pos, 0);

                // update item
                this.updateItem(this.nextInsertItemNo, item.gameObject);
            }
        }

        private void resizeContent()
        {
            var size = this.contentRect.getSize();
            if (this.direction == Direction.Vertical) size.y = this.itemSize * this.totalItemCount;
            else size.x = this.itemSize * this.totalItemCount;
            this.contentRect.setSize(size);
        }
        private void updateItem(int index, GameObject itemObj)
        {

            if (index < 0 || index >= this.totalItemCount)
            {
                itemObj.SetActive(false);
                var item = itemObj.GetComponent<IDynamicScrollViewItem>();
                if (item != null)
                {
                    if (activeFalse != null)
                    {
                        activeFalse(index, item);
                    }
                }
            }
            else
            {

                itemObj.SetActive(true);

                var item = itemObj.GetComponent<IDynamicScrollViewItem>();
                if (item != null)
                {
                    item.onUpdateItem(index);
                    if (itemRenderer != null)
                    {
                        itemRenderer(index, item);
                    }
                }
            }
        }



        [ContextMenu("Initialize")]
        public virtual void init()
        {

            this.clear();

            // [ RectTransform ]

            var rectTransform = this.GetComponent<RectTransform>();
            rectTransform.setFullSize();

            // [ ScrollRect ]
            var scrollRect = this.GetComponent<ScrollRect>();
            scrollRect.horizontal = this.direction == Direction.Horizontal;
            scrollRect.vertical = this.direction == Direction.Vertical;
            scrollRect.scrollSensitivity = 15f;

        }

        protected virtual void clear()
        {

            while (this.transform.childCount > 0)
            {
                DestroyImmediate(this.transform.GetChild(0).gameObject);
            }
        }


        protected abstract float contentAnchoredPosition { get; set; }
        protected abstract float contentSize { get; }
        protected abstract float viewportSize { get; }
        protected abstract float itemSize { get; }


        protected Direction direction = Direction.Vertical;
        protected LinkedList<RectTransform> containers = new LinkedList<RectTransform>();
        protected float prevAnchoredPosition = 0;
        protected int nextInsertItemNo = 0; // item index from left-top of viewport at next insert
        protected int prevTotalItemCount = 99;
        protected ScrollRect scrollRect = null;
        protected RectTransform viewportRect = null;
        protected RectTransform contentRect = null;




        /// <summary> Scroll Direction </summary>
	    public enum Direction
        {
            Vertical,
            Horizontal,
        }
    }
}
