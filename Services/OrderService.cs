using Data.Infrastructure;
using Data.Repositories;
using Models.Models;
using Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Services
{
    public interface IOrderService
    {
        Order Create(Order order);
        List<Order> GetList(string startDate, string endDate, string customerName, string status,
            int pageIndex, int pageSize, out int totalRow);

        Order GetDetail(int orderId);

        OrderDetail CreateDetail(OrderDetail order);

        void DeleteDetail(int productId, int orderId, int colorId, int sizeId);

        void UpdateStatus(int orderId);
        void Delete(int orderId);

        List<OrderDetail> GetOrderDetails(int orderId);

        void Save();
        int Add(AddOrderViewModel orderViewModel);
    }

    public class OrderService : IOrderService
    {
        private IOrderRepository _orderRepository;
        private IOrderDetailRepository _orderDetailRepository;
        private IUnitOfWork _unitOfWork;

        public OrderService(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _unitOfWork = unitOfWork;
        }

        public Order Create(Order order)
        {
            try
            {
                _orderRepository.Add(order);
                _unitOfWork.Commit();
                return order;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int Add(AddOrderViewModel orderViewModel)
        {
            Order order = new Order()
            {
                CustomerName = orderViewModel.CustomerName,
                CustomerAddress = orderViewModel.CustomerAddress,
                CustomerEmail = orderViewModel.CustomerEmail,
                CustomerMobile = orderViewModel.CustomerMobile,
                CustomerMessage = orderViewModel.CustomerMessage,
                PaymentMethod = orderViewModel.PaymentMethod,
                CreatedDate = DateTime.Now
            };
            _orderRepository.Add(order);
            _unitOfWork.Commit();
            var ods = JsonConvert.DeserializeObject<List<OrderDetailViewModel>>(orderViewModel.OrderDetails);
            foreach (var p in ods)
            {
                OrderDetail od = new OrderDetail()
                {
                    ProductID = p.ID,
                    OrderID = order.ID,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    ColorId=1,
                    SizeId=4
                };
                _orderDetailRepository.Add(od);
                _unitOfWork.Commit();
            }
            return 1;
        }

        public void UpdateStatus(int orderId)
        {
            var order = _orderRepository.GetSingleById(orderId);
            order.Status = true;
            _orderRepository.Update(order);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public List<Order> GetList(string startDate, string endDate, string customerName,
            string paymentStatus, int pageIndex, int pageSize, out int totalRow)
        {
            var query = _orderRepository.GetAll();
            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime start = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.CreatedDate >= start);

            }
            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime end = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.CreatedDate <= end);
            }
            if (!string.IsNullOrEmpty(paymentStatus))
                query = query.Where(x => x.PaymentStatus == paymentStatus);
            totalRow = query.Count();
            return query.OrderByDescending(x => x.CreatedDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public Order GetDetail(int orderId)
        {
            return _orderRepository.GetSingleByCondition(x => x.ID == orderId, new string[] { "OrderDetails" });
        }

        public List<OrderDetail> GetOrderDetails(int orderId)
        {
            return _orderDetailRepository.GetMulti(x => x.OrderID == orderId, new string[] { "Order","Color","Size", "Product" }).ToList();
        }

        public OrderDetail CreateDetail(OrderDetail order)
        {
            return _orderDetailRepository.Add(order);
        }

        public void DeleteDetail(int productId, int orderId, int colorId, int sizeId)
        {
            var detail = _orderDetailRepository.GetSingleByCondition(x => x.ProductID == productId
           && x.OrderID == orderId && x.ColorId == colorId && x.SizeId == sizeId);
            _orderDetailRepository.Delete(detail);
        }

        public void Delete(int orderId)
        {
            var detail = _orderDetailRepository.GetSingleByCondition(x => x.OrderID == orderId);
            _orderDetailRepository.Delete(detail);
            var order = _orderRepository.GetSingleById(orderId);
            _orderRepository.Delete(order);
            _unitOfWork.Commit();
        }
    }
}