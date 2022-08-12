using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPFSharpADPS.ViewModels;

namespace TestCoreADPS.TestViewModels
{
    public class MailsFilterViewTestModel : MailsFilterViewModel
    {
        public readonly List<string> OnPropertyCalledArgs = new List<string>();

        private void _onPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyCalledArgs.Add(e.PropertyName);
        }

        public MailsFilterViewTestModel()
        {
            PropertyChanged += _onPropertyChanged;
        }
    }

    [TestClass]
    public class TestMailsFilterViewModel
    {
        private static void _fillValues(MailsFilterViewModel viewModel)
        {
            viewModel.IsNameFilterEnabled = true;
            viewModel.NameFilterValue = "John";

            viewModel.IsAdditionalNotesFilterEnabled = true;
            viewModel.AdditionalNotesFilterValue = "Additional notes";

            viewModel.IsInlineMessageFilterEnabled = true;
            viewModel.InlineMessageFilterValue = "today on 9am";

            viewModel.IsAttachmentFilterEnabled = true;
            viewModel.HashsumValue = "12345abcde";

            viewModel.IsAttachmentSizeFilterEnabled = true;
            viewModel.RawMaxAttachmentSizeValue = "8192";

            viewModel.IsLocationFilterEnabled = true;
            viewModel.RawCoordinatesFilterValue = "12.34,56.78";
            viewModel.RadiusSliderValue = 900;

            viewModel.IsDateTimeFromFilterEnabled = true;
            viewModel.IsDateTimeToFilterEnabled = true;

            viewModel.IsDampingDistanceFilterEnabled = true;
            viewModel.RawPopulationValue = "12304";
        }

        [TestMethod]
        public void TestOk()
        {
            var viewModel = new MailsFilterViewTestModel();
            _fillValues(viewModel);

            Assert.IsNull(viewModel.Result);
            Assert.IsTrue(viewModel.SearchCommand.CanExecute(null));
            viewModel.SearchCommand.Execute(null);
            Assert.IsNotNull(viewModel.Result);

            Assert.AreEqual(MailsFilterDialogResultStatus.NewSearch, viewModel.Result.Status);
            Assert.AreEqual(8, viewModel.Result.Filters.Count);
        }

        [TestMethod]
        public void TestEmpty()
        {
            var viewModel = new MailsFilterViewTestModel();
            _fillValues(viewModel);

            viewModel.IsNameFilterEnabled = false;
            viewModel.IsAdditionalNotesFilterEnabled = false;
            viewModel.IsInlineMessageFilterEnabled = false;
            viewModel.IsAttachmentFilterEnabled = false;
            viewModel.IsAttachmentSizeFilterEnabled = false;
            viewModel.IsLocationFilterEnabled = false;
            viewModel.IsDateTimeFromFilterEnabled = false;
            viewModel.IsDateTimeToFilterEnabled = false;
            viewModel.IsDampingDistanceFilterEnabled = false;

            viewModel.SelectedFilterModeComboboxItem = viewModel.FilterModeComboboxItems.ElementAt(2);

            Assert.IsNull(viewModel.Result);
            Assert.IsTrue(viewModel.SearchCommand.CanExecute(null));
            viewModel.SearchCommand.Execute(null);
            Assert.IsNotNull(viewModel.Result);

            Assert.AreEqual(MailsFilterDialogResultStatus.UniteResults, viewModel.Result.Status);
            Assert.AreEqual(0, viewModel.Result.Filters.Count);
        }

        [TestMethod]
        public void TestInvalidSizeValue()
        {
            var viewModel = new MailsFilterViewTestModel();
            _fillValues(viewModel);
            viewModel.RawMaxAttachmentSizeValue = "0xabcd";

            Assert.IsNull(viewModel.Result);
            Assert.IsFalse(viewModel.IsValidMaxAttachmentSizeValue);
            Assert.IsFalse(viewModel.SearchCommand.CanExecute(null));
        }

        [TestMethod]
        public void TestInvalidCoordinatesValue()
        {
            var viewModel = new MailsFilterViewTestModel();
            _fillValues(viewModel);
            viewModel.RawCoordinatesFilterValue = "Moscow";

            Assert.IsNull(viewModel.Result);
            Assert.IsFalse(viewModel.IsValidCoordinatesFilterValue);
            Assert.IsFalse(viewModel.SearchCommand.CanExecute(null));
        }

        [TestMethod]
        public void TestInvalidPopulationValue()
        {
            var viewModel = new MailsFilterViewTestModel();
            _fillValues(viewModel);
            viewModel.RawPopulationValue = "0b010001001";

            Assert.IsNull(viewModel.Result);
            Assert.IsFalse(viewModel.IsValidPopulationValue);
            Assert.IsFalse(viewModel.SearchCommand.CanExecute(null));
        }
    }
}
