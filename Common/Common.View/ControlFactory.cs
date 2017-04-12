using Common.View.CustomControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Common.View
{
    public static class ControlFactory
    {
        public const string Label_BigAmount = "Label_BigAmount";
        public const string Label_Indicator = "Label_Indicator";
        public const string Label_Subtitle = "Label_Subtitle";
        public const string Label_Field = "Label_Field";
        public const string Label_Icon = "Label_Icon";
        public const string Medium_Label_Icon = "Medium_Label_Icon";
        public const string Small_Label_Icon = "Small_Label_Icon";

        #region DatePicker
        /// <summary>
        /// Creates DatePickerEx with given binding path, maximumDate and format
        /// </summary>
        /// <param name="path"></param>
        /// <param name="maximumDate"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DatePickerEx CreateDatePicker(string path, DateTime maximumDate, string format = null)
        {
            DatePickerEx datePicker = new DatePickerEx
            {
                MaximumDate = maximumDate,
                Format = format
            };
            datePicker.SetBinding(DatePickerEx.DateProperty, path);
            return datePicker;
        }
        #endregion

        #region EditorEx
        public static EditorEx CreateEditor(string path)
        {
            EditorEx editor = new EditorEx();
            editor.SetBinding(EditorEx.TextProperty, path);
            return editor;
        }
        #endregion

        #region EntryEx
        public static EntryEx CreateEntry(string path, BindingMode mode, Keyboard keyboard, string format = null, string placeHolder = null, 
                                          IValueConverter valueConverter = null, object converterParameter = null)
        {
            EntryEx entry = new EntryEx
            {
                Keyboard = keyboard,
                Placeholder = placeHolder
            };

            if (path != null)
            {
                entry.SetBinding(EntryEx.TextProperty, 
                    new Binding(path, mode, valueConverter, converterParameter, format));
            }
            return entry;
        }
        #endregion

        #region Grid
        /// <summary>
        /// Auto fill a formulary view
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="rows"></param>
        /// <param name="maxNumberColumns"></param>
        /// <returns></returns>
        public static Grid AutoFill(Grid grid, object[] rows, int initialColumn, int maxNumberColumns)
        {
            if (grid != null && rows != null)
            {
                grid.RowSpacing = 0;
                for (int rowCount = 0; rowCount < rows.Length; rowCount++)
                {
                    // if row is null, create empty row
                    if (rows[rowCount] == null)
                    {
                        grid = AddRowDefinition(grid, new GridLength(5));
                    }
                    else
                    {
                        grid = AddRowDefinition(grid, GridLength.Auto);

                        Xamarin.Forms.View field = rows[rowCount] as Xamarin.Forms.View;
                        if (field != null)
                        {
                            // if is a field added and expand in all the columns
                            grid.Children.Add(field, initialColumn, initialColumn + maxNumberColumns, rowCount, rowCount + 1);
                        }
                        else
                        {
                            // If is an array of fields, add each fiel in a separate column.
                            Xamarin.Forms.View[] row = rows[rowCount] as Xamarin.Forms.View[];
                            grid = AddFieldsInRow(grid, rowCount, initialColumn, row);
                        }
                    }
                }
            }
            return grid;
        }

        /// <summary>
        /// Add each view in given row count but diferent columns.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="rowCount"></param>
        /// <param name="columCount"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public static Grid AddFieldsInRow(Grid grid, int rowCount, int columCount, Xamarin.Forms.View[] row)
        {
            if (row != null && grid != null)
            {
                int numColumns = grid.ColumnDefinitions.Count;
                for (int column = 0; column < row.Length; column++)
                {
                    if (column >= numColumns)
                    {
                        grid = AddColumnDefinition(grid);
                    }
                    grid.Children.Add(row[column], column + columCount, rowCount);
                }
            }
            return grid;
        }

        public static Grid AddColumnDefinition(Grid grid)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
            return grid;
        }

        public static Grid AddRowDefinition(Grid grid, GridLength height)
        {
            grid.RowDefinitions.Add(new RowDefinition()
            {
                Height = height
            });
            return grid;
        }
        #endregion

        #region Label
        /// <summary>
        /// Creates a label with the binding to the given path 
        /// </summary>
        /// <param name="path">Path used to bind the value to the label</param>
        /// <param name="format">Format applied before render</param>
        /// <param name="style">Valid app style</param>
        /// <param name="valueConverter">Converter used while binding</param>
        /// <param name="converterParameter">parameters for the IValueConverter</param>
        /// <returns></returns>
        public static Label CreateLabel(string path, string format = null, string style = null, IValueConverter valueConverter = null, object converterParameter = null)
        {
            Label label = new Label();
            if (style != null)
            {
                label.Style = (Style)Application.Current.Resources[style.ToString()];
            }

            if (path != null)
            {
                label.SetBinding(Label.TextProperty,
                    new Binding(path, BindingMode.OneWay, valueConverter, converterParameter, format));
            }
            return label;
        }

        /// <summary>
        /// Create a label with the given text and style
        /// </summary>
        /// <param name="text"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static LabelEx CreateStaticLabel(string textLabel, string style = Label_Field)
        {
            LabelEx label = new LabelEx();
            label.Text = textLabel; 
            if (style != null)
            {
                label.Style = (Style)Application.Current.Resources[style.ToString()];
            }

            return label;
        }

        /// <summary>
        /// Create an Icon
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="style">Style of the icon, by default it will be Label_Icon</param>
        /// <returns></returns>
        public static LabelIcon CreateIcon(string icon = null, string style = Label_Icon)
        {
            LabelIcon label = new LabelIcon();
            label.Text = icon;

            if (style != null)
            {
                label.Style = (Style)Application.Current.Resources[style.ToString()];
                if (style == Label_Icon || style == Small_Label_Icon || style == Medium_Label_Icon)
                {
                    label.FontFamily = Device.OnPlatform(
                        "Dynamics Symbol",
                        null, //render
                        "Assets/Fonts/DynamicsSymbol.ttf#Dynamics Symbol"
                    );
                }
            }

            return label;
        }
        #endregion

        #region PickerEx
        public delegate Task<bool> SelectItemAction(string selectedItemKey, int selectedItemIndex);

        /// <summary>
        /// Create a picker with the optional title and selectedIndexChange event method.
        /// </summary>
        /// <param name="titleLabel">Title on top of the list items.</param>
        /// <param name="selectedIndexChangeDelegate">Method to call when the event SelectedItemChanged is triggered on the picker control.</param>
        /// <param name="itemsSource">Items to show in the picker.</param>
        /// <param name="startIndex">Index of the initially selected item.</param>
        /// <returns>A new instance of PickerEx control.</returns>
        public static PickerEx CreatePicker(string titleLabel, SelectItemAction selectedIndexChangeDelegate, IEnumerable itemsSource, int startIndex)
        {
            string title = titleLabel == null ? null : titleLabel;
            PickerEx picker = new PickerEx();

            if (!String.IsNullOrEmpty(title))
            {
                picker.Title = title;
            }

            picker.ItemsSource = itemsSource;

            // Set the initial index before adding a handler.
            picker.SelectedIndex = startIndex;

            if (selectedIndexChangeDelegate != null)
            {
                int previousIndex = picker.SelectedIndex;
                picker.SelectedIndexChanged += async (sender, args) =>
                {
                    if (picker.SelectedIndex > -1 && picker.SelectedIndex != previousIndex)
                    {
                        string key = picker.Items[picker.SelectedIndex];

                        if (await selectedIndexChangeDelegate(key, picker.SelectedIndex))
                        {
                            previousIndex = picker.SelectedIndex;
                        }
                        else
                        {
                            // Set back to previous index
                            picker.SelectedIndex = previousIndex;
                        }
                    }
                };
            }

            return picker;
        }

        /// <summary>
        /// Create a picker with the optional title and selectedIndexChange event method.
        /// </summary>
        /// <param name="titleLabel">Title on top of the list items.</param>
        /// <param name="selectedIndexChangeDelegate">Method to call when the event SelectedItemChanged is triggered on the picker control.</param>
        /// <returns>A new instance of PickerEx control.</returns>
        public static PickerEx CreatePicker(string titleLabel = null, SelectItemAction selectedIndexChangeDelegate = null)
        {
            return ControlFactory.CreatePicker(titleLabel, selectedIndexChangeDelegate, null, 0);
        }

        /// <summary>
        /// Items property is not currently bindable so adding code to manually set items.
        /// When an item is selected will call the delegate method
        /// </summary>
        /// <param name="picker"></param>
        /// <param name="items">List of items to display in the picker, the key is used as display label</param>
        /// <returns></returns>
        public static PickerEx SetItems(PickerEx picker, ICollection<string> items)
        {
            picker.Items.Clear();
            if (items != null)
            {
                foreach (string key in items)
                {
                    picker.Items.Add(key);
                }
            }
 
            return picker;
        }        
        #endregion
    }
}
