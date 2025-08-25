using MassTransitRMQExtensions.Attributes.ConsumerAttributes;
using MassTransitRMQExtensions.Models;
using DeliveryDatePlanning.Application.Declaration.Handler;
using DeliveryDatePlanning.Application.Declaration.Handler.Command;
using DeliveryDatePlanning.Protocol.Models;
using Models;

namespace DeliveryDatePlanning.BackgroundService.Controllers;

public class EncloseStateController
{
    private readonly ICommandHandler handler;

    public EncloseStateController(ICommandHandler handler)
    {
        this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }
    
    [SubscribeBasicOn((int) EncloseState.AcceptedByRegistry)]
    [SubscribeBasicOn((int) EncloseState.FormedForPassingToLogistician)]
    [SubscribeBasicOn((int) EncloseState.WaitingForClientDeliveryToPT)]
    [SubscribeBasicOn((int) EncloseState.ReadyForSending)]
    [SubscribeBasicOn((int) EncloseState.AcceptedByLogistician)]
    [SubscribeBasicOn((int) EncloseState.OnLogisticianKladovka)]
    [SubscribeBasicOn((int) EncloseState.OnRoute)]
    [SubscribeBasicOn((int) EncloseState.OnCourier)]
    [SubscribeBasicOn((int) EncloseState.Unclaimed)]
    [SubscribeBasicOn((int) EncloseState.PassedToReturn)]
    [SubscribeBasicOn((int) EncloseState.PassedToLogistician)]
    [SubscribeBasicOn((int) EncloseState.PassedForOutOfCityDelivery)]
    [SubscribeBasicOn((int) EncloseState.AcceptedForOutOfCityDelivery)]
    [SubscribeBasicOn((int) EncloseState.BaledForOutOfCityDelivery)]
    [SubscribeBasicOn((int) EncloseState.CourierAssigned)]
    [SubscribeBasicOn((int) EncloseState.PassedToDeliveryDepartment)]
    [SubscribeBasicOn((int) EncloseState.Consolidated)]
    [SubscribeBasicOn((int) EncloseState.Canceled)]
    [SubscribeBasicOn((int) EncloseState.PosteRestante)]
    [SubscribeBasicOn((int) EncloseState.PartiallyIssued)]
    [SubscribeBasicOn((int) EncloseState.PartiallyReturned)]
    [SubscribeBasicOn((int) EncloseState.ReturnDeliveredToPT)]
    [SubscribeBasicOn((int) EncloseState.ReturnUnclaimed)]
    [SubscribeBasicOn((int) EncloseState.PulledForStoringAtWarehouse)]
    [SubscribeBasicOn((int) EncloseState.StoredInWarehouse)]
    [SubscribeBasicOn((int) EncloseState.ReadyForPartialReturn)]
    [SubscribeBasicOn((int) EncloseState.PassedToPartialReturn)]
    [SubscribeBasicOn((int) EncloseState.UnderCheckAsPartialInWarehouse)]
    [SubscribeBasicOn((int) EncloseState.CheckedAsPartialInWarehouse)]
    [SubscribeBasicOn((int) EncloseState.Searched)]
    [SubscribeBasicOn((int) EncloseState.Lost)]
    [SubscribeBasicOn((int) EncloseState.RefundedToClient)]
    [SubscribeBasicOn((int) EncloseState.RefundedToClientAndFound)]
    [SubscribeBasicOn((int) EncloseState.SearchedBySecurity)]
    [SubscribeBasicOn((int) EncloseState.ReturnOnCourier)]
    [SubscribeBasicOn((int) EncloseState.ScannedForDelivery)]
    [SubscribeBasicOn((int) EncloseState.ScannedToConsolidate)]
    [SubscribeBasicOn((int) EncloseState.ScannedToRegistry)]
    [SubscribeBasicOn((int) EncloseState.PassedFictionally)]
    [SubscribeBasicOn((int) EncloseState.PassedToInIntraregionalRoute)]
    [SubscribeBasicOn((int) EncloseState.InSearchReturn)]
    [SubscribeBasicOn((int) EncloseState.LostReturn)]
    [SubscribeBasicOn((int) EncloseState.ClientCompensated)]
    [SubscribeBasicOn((int) EncloseState.ClientCompensatedFoundItself)]
    [SubscribeBasicOn((int) EncloseState.ReturnConsolidated)]
    [SubscribeBasicOn((int) EncloseState.OnCourierConsolidated)]
    [SubscribeBasicOn((int) EncloseState.WrittenOffByExpiration)]
    [SubscribeBasicOn((int) EncloseState.DisposedByCompany)]
    public async Task HandleEncloseState(MsgContext<IEnumerable<EventJson>> events)
    {
        var state = (EncloseState) int.Parse(events.Exchange);
        
        foreach (var @event in events.Message)
        {
            await this.handler.Handle(new EncloseStateRecordCommand(@event.Id, @event.Time, state));
        }
    }
}