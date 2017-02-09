using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using BankMVC.BankService;
using BankMVC.Models;

namespace BankMVC.Controllers
{
    public class TransactionController : Controller
    {
        private const int TransactionsToShow = 10;
        private readonly WcfServiceClient _client = new WcfServiceClient();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult History()
        {
            var transactions = _client.GetTransactions(TransactionsToShow);
            var atm = _client.GetAtmState();
            var list = transactions.Select(t => new TransactionViewModel
            {
                IsIncome = t.IsIncome,
                Banknote100 = t.Banknote100,
                Banknote500 = t.Banknote500,
                Banknote1000 = t.Banknote1000,
                Banknote5000 = t.Banknote5000,
                TimeStamp = t.TimeStamp
            }).ToList();
            ViewBag.Account = atm.AccountValue;
            return View(list);
        }

        public ActionResult Atm()
        {
            var atm = _client.GetAtmState();
            var model = new AtmViewModel
            {
                Banknote100 = atm.Banknote100,
                Banknote500 = atm.Banknote500,
                Banknote1000 = atm.Banknote1000,
                Banknote5000 = atm.Banknote5000,
                AccountValue = atm.AccountValue
            };
            return View(model);
        }

        public ActionResult PutMoney()
        {
            var model = new TransactionViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult PutMoney(TransactionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            if (model.TransactionValue < 100)
            {
                ModelState.AddModelError(string.Empty, "Нельзя положить на счет " + model.TransactionValue + "!");
                return View(model);
            }
            var transaction = new Transaction
            {
                Banknote100 = model.Banknote100,
                Banknote500 = model.Banknote500,
                Banknote1000 = model.Banknote1000,
                Banknote5000 = model.Banknote5000
            };
            _client.PutMoneyAsync(transaction);
            var atm = _client.GetAtmState();
            ViewBag.Account = atm.AccountValue;
            ViewBag.Text = "Были внесены следующие банкноты:";
            return View("Details", model);
        }

        public ActionResult GetMoneyValue()
        {
            var atm = _client.GetAtmState();
            ViewBag.Account = atm.AccountValue;
            var model = new GetMoneyViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult GetMoneyValue(GetMoneyViewModel model)
        {
            if (model.Value >= 100)
                return RedirectToAction("GetMoney", new {value = model.Value});
            ModelState.AddModelError(string.Empty, "Нельзя снять со счета " + model.Value + "!");
            return View(model);
        }

        public async Task<ActionResult> GetMoney(int? value)
        {
            if (value == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var transaction = await _client.GetMoneyAsync((int) value, false);
            var model = new TransactionViewModel
            {
                Banknote100 = transaction.Banknote100,
                Banknote500 = transaction.Banknote500,
                Banknote1000 = transaction.Banknote1000,
                Banknote5000 = transaction.Banknote5000
            };
            var atm = _client.GetAtmState();
            ViewBag.Account = atm.AccountValue;
            return View(model);
        }

        [HttpPost, ActionName("GetMoney")]
        public async Task<ActionResult> GetMoneyConfirmed(int? value)
        {
            if (value == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var transaction = await _client.GetMoneyAsync((int) value, true);
            var model = new TransactionViewModel
            {
                Banknote100 = transaction.Banknote100,
                Banknote500 = transaction.Banknote500,
                Banknote1000 = transaction.Banknote1000,
                Banknote5000 = transaction.Banknote5000
            };
            var atm = _client.GetAtmState();
            ViewBag.Account = atm.AccountValue;
            ViewBag.Text = "Были получены следующие банкноты:";
            return View("Details", model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _client.Close();
            base.Dispose(disposing);
        }
    }
}
