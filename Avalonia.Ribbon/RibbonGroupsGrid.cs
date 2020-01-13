﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Collections.Specialized;

namespace Avalonia.Controls.Ribbon
{
    public class RibbonGroupsGrid : Grid
    {
        public RibbonGroupsGrid()
        {
            this.LayoutUpdated += RibbonGroupsGrid_LayoutUpdated2;
            //this.OnSizeChanged
            
            /*if (TemplatedParent is RibbonGroupBox parentBox)
            {
                parentBox.Rearranged += (sneder, args) => ArrangeOverride(Bounds.Size);
                parentBox.Remeasured += (sneder, args) => MeasureOverride(Bounds.Size);
            }*/
        }

        double _lastTotalChildrenWidth = -1;
        bool _cycle2 = false;
        private void RibbonGroupsGrid_LayoutUpdated2(object sender, EventArgs e)
        {
            if ((_cycle2) && (_lastTotalChildrenWidth >= 0))
            {
                Arrange2(Bounds.Size, _lastTotalChildrenWidth);
                _cycle2 = false;
            }
            else
            {
                _lastTotalChildrenWidth = Arrange1(Bounds.Size);
                _cycle2 = true;
                Measure(Bounds.Size);
            }
        }

        protected override void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.ChildrenChanged(sender, e);
            _childrenCountChanged = true;
        }

        protected override void LogicalChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.LogicalChildrenCollectionChanged(sender, e);
            _childrenCountChanged = true;
        }

        private void RibbonGroupsGrid_LayoutUpdated(object sender, EventArgs e)
        {
            OnSizeChanged(Bounds.Size);
        }
        Size OnSizeChanged(Size arrangeSize)
        {
            //Arrange2(Bounds.Size, Arrange1(Bounds.Size));
            return ArrangeOverride(arrangeSize);
        }

        //int _lastChildrenCount = 0;
        bool _childrenCountChanged = true;
        
        double _lastArrangeSizeWidth = -1;
        double _largeTotalChildrenWidth = -1;

        private double Arrange1(Size arrangeSize)
        {
            var children = Children.Where(x => x is RibbonGroupBox).Cast<RibbonGroupBox>();
            double totalChildrenWidth = 0;
            double largeTotalChildrenWidth = 0;
            double smallTotalChildrenWidth = 0;

            if ((_largeTotalChildrenWidth < 0) || _childrenCountChanged)
            {
                _largeTotalChildrenWidth = 0;
                for (int i = 0; i < children.Count(); i++)
                {
                    GroupDisplayMode prevMode = children.ElementAt(i).DisplayMode;
                    children.ElementAt(i).DisplayMode = GroupDisplayMode.Large;
                    _largeTotalChildrenWidth = children.ElementAt(i).Bounds.Width;
                    children.ElementAt(i).DisplayMode = prevMode;
                }
            }
            string columnDefinitions = string.Empty;
            //////GroupDisplayMode[] displayModes = new GroupDisplayMode[children.Count()];
            for (int i = 0; i < children.Count(); i++)
            {
                RibbonGroupBox box = children.ElementAt(i);
                GroupDisplayMode oldMode = box.DisplayMode;
                //GridExtra.Avalonia.GridEx.SetAreaName(box, i.ToString());
                if (_childrenCountChanged)
                    Grid.SetColumn(box, i);
                //////displayModes[i] = box.DisplayMode;

                /*box.DisplayMode = GroupDisplayMode.Large;
                largeTotalChildrenWidth += box.DesiredSize.Width;
                box.DisplayMode = GroupDisplayMode.Small;
                smallTotalChildrenWidth += box.DesiredSize.Width;*/

                //box.Measure(tempSize);
                //box.Arrange(arrangeSize);
                //templateArea += i.ToString() + " ";
                columnDefinitions += "Auto,";
                //box.DisplayMode = oldMode;
                box.DisplayMode = box.DisplayMode;
                totalChildrenWidth += box.Bounds.Width;
            }
            if ((children.Count() > 0) && _childrenCountChanged)
            {
                columnDefinitions = columnDefinitions.Substring(0, columnDefinitions.LastIndexOf(','));
                ColumnDefinitions = new ColumnDefinitions(columnDefinitions);
            }

            /*for (int i = 0; i < children.Count(); i++)
            {
                //if (children.ElementAt(i).DisplayMode != GroupDisplayMode.Large)
                children.ElementAt(i).DisplayMode = displayModes[i];
            }*/

            //templateArea += "/";
            //GridExtra.Avalonia.GridEx.SetTemplateArea(this, templateArea);
            //Size bigSize = base.ArrangeOverride(arrangeSize);
            Debug.WriteLine(totalChildrenWidth + "; " + arrangeSize.Width + "; " + _lastArrangeSizeWidth);
            return totalChildrenWidth;
        }

        private void Arrange2(Size arrangeSize, double totalChildrenWidth)
        {
            var children = Children.Where(x => x is RibbonGroupBox).Cast<RibbonGroupBox>();
            double largeTotalChildrenWidth = 0;
            double smallTotalChildrenWidth = 0;

            Debug.WriteLine("2: " + totalChildrenWidth + "; " + arrangeSize.Width + "; " + _lastArrangeSizeWidth);

            if (_lastArrangeSizeWidth >= 0)
            {
                if ((totalChildrenWidth > arrangeSize.Width) && (arrangeSize.Width < _lastArrangeSizeWidth))
                {
                    //for (int i = children.Count() - 1; i >= 0; i--)
                    foreach (RibbonGroupBox box in children.Where(x => x.DisplayMode == GroupDisplayMode.Large).Reverse())
                    {
                        //RibbonGroupBox box = children.ElementAt(i);
                        double newTotalChildrenWidth = 0;
                        //base.MeasureCore(arrangeSize);
                        for (int j = 0; j < children.Count(); j++)
                        {
                            RibbonGroupBox box2 = children.ElementAt(j);
                            box2.InvalidateArrange();
                            box2.InvalidateMeasure();
                            box2.Measure(arrangeSize);
                            //box2.DisplayMode = box2.DisplayMode;
                            //box.Arrange();
                            //box2.InvalidateArrange();
                            //box2.InvalidateMeasure();
                            //box2.Arrange(tempRect);
                            //box2.Measure(tempSize);
                            newTotalChildrenWidth += box2.Bounds.Width;
                        }
                        Debug.WriteLine("newTotalChildrenWidth: " + newTotalChildrenWidth);
                        if (GetChildrenTotalWidthForDisplayMode((GroupDisplayMode)(-1)) <= arrangeSize.Width)
                            break;
                        else
                        {
                            box.DisplayMode = GroupDisplayMode.Small;
                        }
                        break;
                    }
                }
                else if ((totalChildrenWidth <= arrangeSize.Width) && (arrangeSize.Width > _lastArrangeSizeWidth))
                {
                    //for (int i = 0; i < children.Count; i++)

                    //for (int i = 0; i < children.Count(); i++)
                    foreach (RibbonGroupBox box in children.Where(x => x.DisplayMode != GroupDisplayMode.Large))
                    {
                        //RibbonGroupBox box = children.ElementAt(i);
                        box.DisplayMode = GroupDisplayMode.Large;
                        double newTotalChildrenWidth = 0;
                        //base.MeasureCore(arrangeSize);
                        for (int j = 0; j < children.Count(); j++)
                        {
                            RibbonGroupBox box2 = children.ElementAt(j);
                            box2.InvalidateArrange();
                            box2.InvalidateMeasure();
                            box2.Measure(arrangeSize);
                            //box2.DisplayMode = box2.DisplayMode;
                            //box2.Measure(tempSize);
                            /*box2.InvalidateArrange();
                            box2.InvalidateMeasure();*/
                            //box2.Arrange(tempRect);
                            //box2.Measure(tempSize);
                            newTotalChildrenWidth += box2.Bounds.Width;
                        }

                        Debug.WriteLine("newTotalChildrenWidth: " + newTotalChildrenWidth);
                        if (GetChildrenTotalWidthForDisplayMode((GroupDisplayMode)(-1)) > arrangeSize.Width)
                        {
                            box.DisplayMode = GroupDisplayMode.Small;
                            break;
                        }
                        break;
                    }
                }
            }
            //_lastTotalChildrenWidth = totalChildrenWidth;
            _lastArrangeSizeWidth = arrangeSize.Width;
            _childrenCountChanged = true;
        }

        double GetChildrenTotalWidthForDisplayMode(GroupDisplayMode mode)
        {
            var children = Children.Where(x => x is RibbonGroupBox).Cast<RibbonGroupBox>();
            double totalWidth = 0;
            GroupDisplayMode[] oldModes = new GroupDisplayMode[children.Count()];

            for (int i = 0; i < children.Count(); i++)
            {
                oldModes[i] = children.ElementAt(i).DisplayMode;
                if ((int)mode >= 0)
                    children.ElementAt(i).DisplayMode = mode;
            }
            Arrange(new Rect(DesiredSize));
            Measure(DesiredSize);
            for (int i = 0; i < children.Count(); i++)
            {
                totalWidth += children.ElementAt(i).Bounds.Width;
                children.ElementAt(i).DisplayMode = oldModes[i];
            }

            return totalWidth;
        }

        private Size zArrangeOverride(Size arrangeSize)
        {
            var children = Children.Where(x => x is RibbonGroupBox).Cast<RibbonGroupBox>();
            double totalChildrenWidth = 0;
            double largeTotalChildrenWidth = 0;
            double smallTotalChildrenWidth = 0;

            if ((_largeTotalChildrenWidth < 0) || _childrenCountChanged)
            {
                _largeTotalChildrenWidth = 0;
                for (int i = 0; i < children.Count(); i++)
                {
                    GroupDisplayMode prevMode = children.ElementAt(i).DisplayMode;
                    children.ElementAt(i).DisplayMode = GroupDisplayMode.Large;
                    _largeTotalChildrenWidth = children.ElementAt(i).Bounds.Width;
                    children.ElementAt(i).DisplayMode = prevMode;
                }
            }

            //Size tempSize = new Size(100, Bounds.Height);
            //Rect tempRect = new Rect(0, 0, tempSize.Width, tempSize.Height);
            //string templateArea = string.Empty;
            string columnDefinitions = string.Empty;
            //////GroupDisplayMode[] displayModes = new GroupDisplayMode[children.Count()];
            for (int i = 0; i < children.Count(); i++)
            {
                RibbonGroupBox box = children.ElementAt(i);
                GroupDisplayMode oldMode = box.DisplayMode;
                //GridExtra.Avalonia.GridEx.SetAreaName(box, i.ToString());
                if (_childrenCountChanged)
                    Grid.SetColumn(box, i);
                //////displayModes[i] = box.DisplayMode;
                
                /*box.DisplayMode = GroupDisplayMode.Large;
                largeTotalChildrenWidth += box.DesiredSize.Width;
                box.DisplayMode = GroupDisplayMode.Small;
                smallTotalChildrenWidth += box.DesiredSize.Width;*/
                
                //box.Measure(tempSize);
                //box.Arrange(arrangeSize);
                //templateArea += i.ToString() + " ";
                columnDefinitions += "Auto,";
                //box.DisplayMode = oldMode;
                box.DisplayMode = box.DisplayMode;
                totalChildrenWidth += box.Bounds.Width;
            }
            if ((children.Count() > 0) && _childrenCountChanged)
            {
                columnDefinitions = columnDefinitions.Substring(0, columnDefinitions.LastIndexOf(','));
                ColumnDefinitions = new ColumnDefinitions(columnDefinitions);
            }

            /*for (int i = 0; i < children.Count(); i++)
            {
                //if (children.ElementAt(i).DisplayMode != GroupDisplayMode.Large)
                children.ElementAt(i).DisplayMode = displayModes[i];
            }*/

            //templateArea += "/";
            //GridExtra.Avalonia.GridEx.SetTemplateArea(this, templateArea);
            //Size bigSize = base.ArrangeOverride(arrangeSize);
            Debug.WriteLine(totalChildrenWidth + "; " + arrangeSize.Width + "; " + _lastArrangeSizeWidth);

            if (_lastArrangeSizeWidth >= 0)
            {
                if ((totalChildrenWidth > arrangeSize.Width) && (arrangeSize.Width < _lastArrangeSizeWidth))
                {
                    for (int i = children.Count() - 1; i >= 0; i--)
                    {
                        RibbonGroupBox box = children.ElementAt(i);
                        double newTotalChildrenWidth = 0;
                        //base.MeasureCore(arrangeSize);
                        for (int j = 0; j < children.Count(); j++)
                        {
                            RibbonGroupBox box2 = children.ElementAt(j);
                            box2.InvalidateArrange();
                            box2.InvalidateMeasure();
                            box2.Measure(arrangeSize);
                            //box2.DisplayMode = box2.DisplayMode;
                            //box.Arrange();
                            //box2.InvalidateArrange();
                            //box2.InvalidateMeasure();
                            //box2.Arrange(tempRect);
                            //box2.Measure(tempSize);
                            newTotalChildrenWidth += box2.Bounds.Width;
                        }
                        Debug.WriteLine("newTotalChildrenWidth: " + newTotalChildrenWidth);
                        if (newTotalChildrenWidth <= arrangeSize.Width)
                            break;
                        else
                        {
                            box.DisplayMode = GroupDisplayMode.Small;
                        }
                    }
                }
                else if ((totalChildrenWidth <= arrangeSize.Width) && (arrangeSize.Width > _lastArrangeSizeWidth))
                {
                    //for (int i = 0; i < children.Count; i++)

                    for (int i = 0; i < children.Count(); i++)
                    {
                        RibbonGroupBox box = children.ElementAt(i);
                        box.DisplayMode = GroupDisplayMode.Large;
                        double newTotalChildrenWidth = 0;
                        //base.MeasureCore(arrangeSize);
                        for (int j = 0; j < children.Count(); j++)
                        {
                            RibbonGroupBox box2 = children.ElementAt(j);
                            box2.InvalidateArrange();
                            box2.InvalidateMeasure();
                            box2.Measure(arrangeSize);
                            //box2.DisplayMode = box2.DisplayMode;
                            //box2.Measure(tempSize);
                            /*box2.InvalidateArrange();
                            box2.InvalidateMeasure();*/
                            //box2.Arrange(tempRect);
                            //box2.Measure(tempSize);
                            newTotalChildrenWidth += box2.Bounds.Width;
                        }

                        Debug.WriteLine("newTotalChildrenWidth: " + newTotalChildrenWidth);
                        if (newTotalChildrenWidth >= arrangeSize.Width)
                        {
                            box.DisplayMode = GroupDisplayMode.Small;
                            break;
                        }
                    }
                }
            }
            //_lastTotalChildrenWidth = totalChildrenWidth;
            _lastArrangeSizeWidth = arrangeSize.Width;

            if (false)
            {
                if ((largeTotalChildrenWidth <= arrangeSize.Width) && (smallTotalChildrenWidth >= arrangeSize.Width))
                {
                    for (int i = children.Count() - 1; i >= 0; i--)
                    {
                        RibbonGroupBox box = children.ElementAt(i);
                        box.DisplayMode = GroupDisplayMode.Small;
                        double newTotalChildrenWidth = 0;
                        for (int j = 0; j < children.Count(); j++)
                            newTotalChildrenWidth += box.DesiredSize.Width;
                        if (newTotalChildrenWidth < arrangeSize.Width)
                            break;
                    }
                }
                else if (largeTotalChildrenWidth <= arrangeSize.Width)
                {
                    for (int i = 0; i < children.Count(); i++)
                        children.ElementAt(i).DisplayMode = GroupDisplayMode.Large;
                }
                else if (largeTotalChildrenWidth >= arrangeSize.Width)
                {
                    for (int i = children.Count() - 1; i >= 0; i--)
                    {
                        children.ElementAt(i).DisplayMode = GroupDisplayMode.Small;
                        //children.ElementAt(i).Arrange(new Rect());
                        ////children.ElementAt(i).Measure(tempSize);
                        //children.ElementAt(i).Measure(arrangeSize);
                        //children.ElementAt(i).Arrange(arrangeSize);
                        /*double newTotalChildrenWidth = 0;
                        for (int j = 0; j < children.Count(); j++)
                            newTotalChildrenWidth += children.ElementAt(i).DesiredSize.Width;

                        if (newTotalChildrenWidth <= arrangeSize.Width)
                            break;*/
                    }
                }
            }
            /*if (DisplayMode == GroupDisplayMode.Large)
            {
                
            }*/

            //_lastChildrenCount = children.Count();
            _childrenCountChanged = true;
            try
            {
                return base.ArrangeOverride(arrangeSize);
            }
            catch (NullReferenceException nex)
            {
                return arrangeSize;
            }
        }
    }
}
