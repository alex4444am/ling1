using ConsoleApp.Model;
using ConsoleApp.Model.Enum;
using ConsoleApp.OutputTypes;

namespace ConsoleApp;

public class QueryHelper : IQueryHelper
{
    /// <summary>
    /// Get Deliveries that has payed
    /// </summary>
    public IEnumerable<Delivery> Paid(IEnumerable<Delivery> deliveries) => deliveries.Where(d => d.IsPaid);//TODO: Завдання 1

    /// <summary>
    /// Get Deliveries that now processing by system (not Canceled or Done)
    /// </summary>
    public IEnumerable<Delivery> NotFinished(IEnumerable<Delivery> deliveries) =>
    deliveries.Where(d => d.Status != DeliveryStatus.Canceled && d.Status != DeliveryStatus.Done); //TODO: Завдання 2
    
    /// <summary>
    /// Get DeliveriesShortInfo from deliveries of specified client
    /// </summary>
    public IEnumerable<DeliveryShortInfo> DeliveryInfosByClient(IEnumerable<Delivery> deliveries, string clientId) =>
    deliveries.Where(d => d.ClientId == clientId)
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
              });//TODO: Завдання 3
    
    /// <summary>
    /// Get first ten Deliveries that starts at specified city and have specified type
    /// </summary>
    public IEnumerable<Delivery> DeliveriesByCityAndType(IEnumerable<Delivery> deliveries, string cityName, DeliveryType type) =>
    deliveries.Where(d => d.StartCity == cityName && d.Type == type)
              .Take(250);//TODO: Завдання 4
    
    /// <summary>
    /// Order deliveries by status, then by start of loading period
    /// </summary>
    public IEnumerable<Delivery> OrderByStatusThenByStartLoading(IEnumerable<Delivery> deliveries) =>
    deliveries.OrderBy(d => d.Status)
              .ThenBy(d => d.StartLoading);//TODO: Завдання 5

    /// <summary>
    /// Count unique cargo types
    /// </summary>
    public int CountUniqCargoTypes(IEnumerable<Delivery> deliveries) =>
    deliveries.Select(d => d.CargoType)
              .Distinct()
              .Count(); //TODO: Завдання 6
    
    /// <summary>
    /// Group deliveries by status and count deliveries in each group
    /// </summary>
    public Dictionary<DeliveryStatus, int> CountsByDeliveryStatus(IEnumerable<Delivery> deliveries) =>
    deliveries.GroupBy(d => d.Status)
              .ToDictionary(g => g.Key, g => g.Count());//TODO: Завдання 7
    
    /// <summary>
    /// Group deliveries by start-end city pairs and calculate average gap between end of loading period and start of arrival period (calculate in minutes)
    /// </summary>
    public IEnumerable<AverageGapsInfo> AverageTravelTimePerDirection(IEnumerable<Delivery> deliveries)
{    return deliveries.GroupBy(d => new { d.StartCity, d.EndCity })
                     .Select(g => new AverageGapsInfo
                     {
                         StartCity = g.Key.StartCity,
                         EndCity = g.Key.EndCity,
                         AverageGapMinutes = g.Average(d => (d.Arrival - d.EndLoading).TotalMinutes)
                     });}//TODO: Завдання 8

    /// <summary>
    /// Paging helper
    /// </summary>
    public IEnumerable<TElement> Paging<TElement, TOrderingKey>(IEnumerable<TElement> elements,
    Func<TElement, TOrderingKey> ordering,
    Func<TElement, bool>? filter = null,
    int countOnPage = 100,
    int pageNumber = 1)
{
    var query = elements;
    
    if (filter != null)
        query = query.Where(filter);

    return query.OrderBy(ordering)
                .Skip((pageNumber - 1) * countOnPage)
                .Take(countOnPage);
}
 //TODO: Завдання 9 
}
