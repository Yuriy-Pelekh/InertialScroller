using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace InertialScroller
{
    public class InertialScroller : ContentControl
    {
        public InertialScroller()
            : base()
        {
            this.DefaultStyleKey = typeof(InertialScroller);
            doubleAnimation = new DoubleAnimation();
            doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("OffsetValue"));
            Storyboard.SetTarget(doubleAnimation, this);

            doubleAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };

            storyBoard.Children.Add(doubleAnimation);

            storyBoard.Completed += new EventHandler(storyBoard_Completed);
        }

        void storyBoard_Completed(object sender, EventArgs e)
        {
            if (OffsetValue % offset != 0)
            {
                var temp = Math.Round(OffsetValue / offset) * offset;
                this.SetValue(OffsetValueProperty, temp);
            }

            isAnimationRunning = false;

            if (isRightButtonClicked && rightButtonClickedCount > 0)
            {
                rightButton_Click(null, null);
                rightButtonClickedCount--;

                if (rightButtonClickedCount == 0)
                    isRightButtonClicked = false;
            }
            else if (isLeftButtonClicked && leftButtonClickedCount > 0)
            {
                leftButton_Click(null, null);
                leftButtonClickedCount--;

                if (leftButtonClickedCount == 0)
                    isLeftButtonClicked = false;
            }
        }

        //private object content;
        private DoubleAnimation doubleAnimation;
        //private Rectangle r = new Rectangle();
        Storyboard storyBoard = new Storyboard();
        //Storyboard boardMinus = new Storyboard();
        private ScrollViewer scrollViwer;

        public static DependencyProperty OffsetValueProperty = DependencyProperty.Register("OffsetValue", typeof(double), typeof(InertialScroller), new PropertyMetadata(OnOffsetValueChanged));

        private static void OnOffsetValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var scroller = (InertialScroller)o;
            
            if (scroller != null)
                scroller.OffsetValue = (double)e.NewValue;
        }

        public double OffsetValue
        {
            get { return (double)this.GetValue(OffsetValueProperty); }
            set
            {
                if (scrollViwer != null)
                {
                    scrollViwer.ScrollToHorizontalOffset(value);
                    
                    if (OffsetValue != scrollViwer.HorizontalOffset)
                        this.SetValue(OffsetValueProperty, scrollViwer.HorizontalOffset);
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var leftButton = GetTemplateChild("HorizontalSmallDecrease") as RepeatButton;
            leftButton.Click += new RoutedEventHandler(leftButton_Click);


            var rightButton = GetTemplateChild("HorizontalSmallIncrease") as RepeatButton;
            rightButton.Click += new RoutedEventHandler(rightButton_Click);

            scrollViwer = GetTemplateChild("Scroll") as ScrollViewer;

            //scrollViwer.MouseLeftButtonDown += new MouseButtonEventHandler(scroll_MouseLeftButtonDown);
        }

        //void scroll_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    MessageBox.Show("Here");
        //}

        public void ScrollToRightEnd()
        {
            doubleAnimation.To = scrollViwer.ExtentWidth;
            storyBoard.Begin();
        }

        void rightButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isAnimationRunning)
            {
                isAnimationRunning = true;
                doubleAnimation.To = OffsetValue + offset;
                storyBoard.Begin();
            }
            else if (rightButtonClickedCount < MaxHistory && !isLeftButtonClicked) //if (!(clickLeftButton && clickRightButton))
            {
                isRightButtonClicked = true;
                rightButtonClickedCount++;
            }
        }

        void leftButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isAnimationRunning)
            {
                isAnimationRunning = true;
                doubleAnimation.To = OffsetValue - offset;
                storyBoard.Begin();
            }
            else if (leftButtonClickedCount < MaxHistory && !isRightButtonClicked) //if (!(clickLeftButton && clickRightButton))
            {
                isLeftButtonClicked = true;
                leftButtonClickedCount++;
            }
        }

        private readonly int MaxHistory = 5;
        private readonly int offset = 450;
        private bool isAnimationRunning;
        private bool isLeftButtonClicked;
        private bool isRightButtonClicked;
        private int leftButtonClickedCount;
        private int rightButtonClickedCount;
    }
}
