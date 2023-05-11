using Adaptive.Aeron;
using Adaptive.Aeron.LogBuffer;
using Adaptive.Agrona;
using Adaptive.Cluster.Codecs;
using Adaptive.Cluster.Service;
using Akka.Actor;

namespace Gateway.Service;

public class AeronGatewayService : IClusteredService
{
    private readonly ILogger<AeronGatewayService> _logger;
    private readonly ActorSystem _actorSystem;
    private ICluster _cluster;

    public AeronGatewayService(ILogger<AeronGatewayService> logger, ActorSystem actorSystem) : base()
    {
        _logger = logger;
        _actorSystem = actorSystem;
    }

    public void OnStart(ICluster cluster, Image snapshotImage)
    {
        _logger.LogInformation("OnStart");
        _cluster = cluster;
    }

    public void OnSessionOpen(IClientSession session, long timestampMs)
    {
        _logger.LogInformation($"OnSessionOpen: sessionId={session.Id}, timestamp={timestampMs}");
    }

    public void OnSessionClose(IClientSession session, long timestampMs, CloseReason closeReason)
    {
        _logger.LogInformation($"OnSessionClose: sessionId={session.Id}, timestamp={timestampMs}");
    }

    public void OnSessionMessage(IClientSession session, long timestampMs, IDirectBuffer buffer, int offset, int length,
        Header header)
    {
        _logger.LogInformation($"OnSessionMessage: sessionId={session.Id}, timestamp={timestampMs}, length={length}");

        _logger.LogInformation("Received Message: " + buffer.GetStringWithoutLengthUtf8(offset, length));

        while (session.Offer(buffer, offset, length) <= 0)
        {
            _cluster.IdleStrategy().Idle();
        }
    }

    public void OnTimerEvent(long correlationId, long timestampMs)
    {
        _logger.LogInformation($"OnTimerEvent: correlationId={correlationId}, timestamp={timestampMs}");
    }

    public void OnTakeSnapshot(ExclusivePublication snapshotPublication)
    {
        _logger.LogInformation("OnTakeSnapshot");
    }

    public void OnRoleChange(ClusterRole newRole)
    {
        _logger.LogInformation($"OnRoleChange: newRole={newRole}");
    }

    public void OnTerminate(ICluster cluster)
    {
        _logger.LogInformation("OnTerminate");
    }

    public void OnNewLeadershipTermEvent(
        long leadershipTermId,
        long logPosition,
        long timestamp,
        long termBaseLogPosition,
        int leaderMemberId,
        int logSessionId,
        ClusterTimeUnit timeUnit,
        int appVersion)
    {
        _logger.LogInformation($"OnNewLeadershipTerm: leadershipTermId={leadershipTermId}");
    }

    public int DoBackgroundWork(long nowNs)
    {
        return 0;
    }
}