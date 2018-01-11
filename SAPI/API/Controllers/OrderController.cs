using API.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [Route("order")]
    public class OrderController : BaseController
    {
        private Repositories.Mongo.Order OrderRepo { get; set; }

        private Repositories.Mongo.Token SessionRepo { get; set; }

        public OrderController(IOptionsSnapshot<Mongo> options, IOptionsSnapshot<Configs.JWT> secret, IHttpContextAccessor httpContextAccessor) : base(options, secret, httpContextAccessor)
        {
            OrderRepo = new Repositories.Mongo.Order(MongoClient);
            SessionRepo = new Repositories.Mongo.Token(MongoClient);
        }

        [HttpGet("all")]
        public DTO.Messages.Wrapper ViewUserOrders()
        {
            var Result = AuthorizeResponse();

            if (Result.Messages.Count > 0)
            {
                Result.Status = "";
                Result.Code = 400;
                return Result;
            }

            var Orders = OrderRepo.QueryableCollection.Where(x => x.OrdererId == Token.sub);

            if (Orders.Count() == 0)
            {
                Result.Messages.Add("OrderList", "is empty");
            }

            Result.Data = Orders;
            return Result;
        }

        [HttpPost("all")]
        public DTO.Messages.Wrapper FilterOrders([FromBody] DTO.Messages.FilterOrders Filter = null)
        {
            var Result = AuthorizeResponse(new HashSet<string> { "Administrator" });

            if (Filter == null)
            {
                Result.Messages.Add("PostBody", "mustn't be empty");
            }
            if (string.IsNullOrWhiteSpace(Filter.Stage))
            {
                Result.Messages.Add("Stage", "mustn't be empty");
            }

            if (Result.Messages.Count > 0)
            {
                Result.Status = "Bad Request";
                Result.Code = 400;

                return Result;
            }
            var Orders = OrderRepo.QueryableCollection;
            switch (Filter.Stage.ToLower())
            {
                case "received": // cần xử lí
                    Orders = Orders.Where(x => x.OrderStatus.Received == null);
                    break;

                case "prepared": // cần chuẩn bị
                    Orders = Orders.Where(x => x.OrderStatus.Prepared == null && x.OrderStatus.Received != null);

                    if (Filter.Queued)
                    {
                        Orders = Orders.Where(x => x.OrderStatus.Received.Who == Token.sub);
                    }
                    break;

                case "delivering": // cần vận chuyển
                    Orders = Orders.Where(x => x.OrderStatus.Delivering == null && x.OrderStatus.Prepared != null);

                    if (Filter.Queued)
                    {
                        Orders = Orders.Where(x => x.OrderStatus.Prepared.Who == Token.sub);
                    }
                    break;

                case "fulfilled": // cần giao hàng
                    Orders = Orders.Where(x => x.OrderStatus.FulFilled == null && x.OrderStatus.Delivering != null);

                    if (Filter.Queued)
                    {
                        Orders = Orders.Where(x => x.OrderStatus.Delivering.Who == Token.sub);
                    }
                    break;

                case "canceled": // đã hủy
                    Orders = Orders.Where(x => x.OrderStatus.Canceled != null);

                    if (Filter.Queued)
                    {
                        Orders = Orders.Where(x => x.OrderStatus.Canceled.Who == Token.sub);
                    }
                    break;

                default:
                    Orders = null;
                    Result.Messages.Add("Stage", "invalid stage");
                    Result.Code = 400;
                    Result.Status = "Bad Request";
                    break;
            }

            Result.Data = Orders;
            return Result;
        }

        [HttpPost("cancel/{id}")]
        public DTO.Messages.Wrapper CancelOrder(string id = "")
        {
            var Result = AuthorizeResponse();

            if (string.IsNullOrWhiteSpace(id))
            {
                Result.Messages.Add("id", "can't be empty");
            }

            if (Result.Messages.Count > 0)
            {
                Result.Status = "Bad Request";
                Result.Code = 400;

                return Result;
            }

            var Order = OrderRepo.QueryableCollection.Where(x => x.Id == id && x.OrdererId == Token.sub);
            if (Order.Count() == 0)
            {
                Result.Messages.Add("Order", "not found, or not on your list");
            }
            else
            {
                var Modifying = Order.First();
                var Change = Modifying.OrderStatus.ProceedStage("Canceled", new DTO.Databases.Flag
                {
                    Who = Token.sub,
                    Why = "User's action"
                });

                if (!Change)
                {
                    Result.Messages.Add("Order", "can't be canceled");
                }
                else
                {
                    OrderRepo.Save(Modifying);
                }
                Result.Messages.Add("Order", "is canceled");
            }

            Result.Data = id;
            return Result;
        }

        [HttpPost("flag")]
        public DTO.Messages.Wrapper SetFlags([FromBody] DTO.Messages.FlagSetter Flagged = null)
        {
            var Result = AuthorizeResponse(new HashSet<string> { "Administrator" });

            if (Flagged == null)
            {
                Result.Messages.Add("PostBody", "can't be empty");
            }
            if (Flagged.Orders == null || Flagged.Orders.Count == 0)
            {
                Result.Messages.Add("Orders", "can't be empty");
            }
            if (string.IsNullOrWhiteSpace(Flagged.Stage))
            {
                Result.Messages.Add("Stage", "can't be empty");
            }
            if (string.IsNullOrWhiteSpace(Flagged.Reason))
            {
                Result.Messages.Add("Reason", "can't be empty");
            }

            if (Result.Messages.Count > 0)
            {
                Result.Status = "Bad Request";
                Result.Code = 400;

                return Result;
            }

            var Orders = OrderRepo.QueryableCollection.Where(x => Flagged.Orders.Contains(x.Id)).ToList();
            var Changes = new Dictionary<string, bool>();

            foreach (var Order in Orders)
            {
                var Changed = Order.OrderStatus.ProceedStage(Flagged.Stage, new DTO.Databases.Flag
                {
                    Who = Token.sub,
                    Why = Flagged.Reason
                });

                if (Changed)
                {
                    OrderRepo.Save(Order);
                }
                Changes.Add(Order.Id, Changed);
            }

            Result.Data = Changes;
            return Result;
        }
    }
}