using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace QSlice {
	class LimitedItemsGridPresenter : DataGridRowsPresenter {
		protected override Visual GetVisualChild(int index) {
			if(MaxCount > 0 && index > MaxCount)
				return null;

			return base.GetVisualChild(index);
		}

		public int MaxCount {
			get { return (int)GetValue(MaxCountProperty); }
			set { SetValue(MaxCountProperty, value); }
		}

		public static readonly DependencyProperty MaxCountProperty =
			DependencyProperty.Register(nameof(MaxCount), typeof(int), typeof(LimitedItemsGridPresenter), new PropertyMetadata(-1));


	}
}
