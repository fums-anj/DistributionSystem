using BH.Models.AccountManagement;
using BH.Models.CustomerManagement;
using System.ComponentModel.DataAnnotations;


namespace BH.Models.ViewModels
{
	public class ShopCustomerVM
	{
		public ShopCustomer Customer { get; set; }
		[Display(Name = "Last Recovery")]
		[DataType(DataType.Date)]
		public DateTime? LastReceivedDate { get; set; }
		[Display(Name = "Last Sale")]
		[DataType(DataType.Date)]
		public DateTime? LastStockTransferDate { get; set; }
		public DateTime? largerDate { get; set; }
		public int days { get; set; }
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		public AccountReceivable? AccountReceivable { get; set; }
		public double? LastRecovery { get; set; }
	}

}
