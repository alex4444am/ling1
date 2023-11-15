using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApp.Model;
using ConsoleApp.Model.Enum;
using ConsoleApp.OutputTypes;

namespace ConsoleApp
{
    public class QueryHelper : IQueryHelper
    {
        public IEnumerable<Delivery> Paid(IEnumerable<Delivery> deliveries) => 
            deliveries.Where(x => x.PaymentId != null);

        public IEnumerable<Delivery> NotFinished(IEnumerable<Delivery> deliveries) => 
            deliveries.Where(x => x.Status != DeliveryStatus.Cancelled && x.Status != DeliveryStatus.Done);

        public IEnumerable<DeliveryShortInfo> DeliveryInfosByClient(IEnumerable<Delivery> deliveries, string clientId) => 
            deliveries
                .Where(x => x.ClientId == clientId)
                .Select(d => new DeliveryShortInfo
                {
                    Id = d.Id,
                    StartCity = d.Direction.Origin.City,
                    EndCity = d.Direction.Destination.City,
                    ClientId = d.ClientId,
                    Type = d.Type,
                    LoadingPeriod = d.LoadingPeriod,
                    ArrivalPeriod = d.ArrivalPeriod,
                    Status = d.Status,
                    CargoType = d.CargoType
                });

        public IEnumerable<Delivery> DeliveriesByCityAndType(IEnumerable<Delivery> deliveries, string cityName, DeliveryType type) => 
            deliveries.Where(x => x.Type == type && x.Direction.Origin.City == cityName)
                      .Take(10);

        public IEnumerable<Delivery> OrderByStatusThenByStartLoading(IEnumerable<Delivery> deliveries) => 
            deliveries.OrderBy(x => x.Status)
                      .ThenBy(x => x.LoadingPeriod.Start);

        public int CountUniqCargoTypes(IEnumerable<Delivery> deliveries) => 
            deliveries.Select(x => x.CargoType)
                      .Distinct()
                      .Count();

        public Dictionary<DeliveryStatus, int> CountsByDeliveryStatus(IEnumerable<Delivery> deliveries) => 
            deliveries.GroupBy(x => x.Status)
                      .ToDictionary(group => group.Key, group => group.Count());

        public IEnumerable<AverageGapsInfo> AverageTravelTimePerDirection(IEnumerable<Delivery> deliveries) => 
            deliveries.GroupBy(d => new { StartCity = d.Direction.Origin.City, EndCity = d.Direction.Destination.City })
                      .Select(group => new AverageGapsInfo
                      {
                          StartCity = group.Key.StartCity,
                          EndCity = group.Key.EndCity,
                          AverageGap = group.Average(delivery => (delivery.ArrivalPeriod.Start.Value - delivery.LoadingPeriod.End.Value).Minutes)
                      });

        public IEnumerable<TElement> Paging<TElement, TOrderingKey>(IEnumerable<TElement> elements,
            Func<TElement, TOrderingKey> ordering,
            Func<TElement, bool>? filter = null,
            int countOnPage = 100,
            int pageNumber = 1) => 
            elements.Where(filter ?? (_ => true))
                    .OrderBy(ordering)
                    .Skip(countOnPage * (pageNumber - 1))
                    .Take(countOnPage);
    }
}
