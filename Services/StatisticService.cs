using Common.ViewModels;
using Data.Repositories;
using System.Collections.Generic;

namespace Services
{
    public interface IStatisticService
    {
        IEnumerable<RevenueStatisticViewModel> GetRevenueStatisticByDate(string fromDate, string toDate);
        IEnumerable<RevenueStatisticMonthViewModel> GetRevenueStatisticByMonth(string fromDate, string toDate);
        IEnumerable<RevenueStatisticYearViewModel> GetRevenueStatisticByYear(string fromDate, string toDate);
    }

    public class StatisticService : IStatisticService
    {
        private IOrderRepository _orderRepository;

        public StatisticService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public IEnumerable<RevenueStatisticViewModel> GetRevenueStatisticByDate(string fromDate, string toDate)
        {
            return _orderRepository.GetRevenueStatisticByDate(fromDate, toDate);
        }

        public IEnumerable<RevenueStatisticMonthViewModel> GetRevenueStatisticByMonth(string fromDate, string toDate)
        {
            return _orderRepository.GetRevenueStatisticByMonth(fromDate, toDate);
        }

        public IEnumerable<RevenueStatisticYearViewModel> GetRevenueStatisticByYear(string fromDate, string toDate)
        {
            return _orderRepository.GetRevenueStatisticByYear(fromDate, toDate);
        }
    }
}