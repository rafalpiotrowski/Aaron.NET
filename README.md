# Aaron.NET
Low Latency Stock Exchange written in C#, backed by [Akka.NET](https://getakka.net/)

# Running in Docker

1. [create docker shared network](#shared-docker-network)
2. [run observability containers](#observability)
3. [build solution images](#running-the-solution)
4. [run solution containers](#running-the-solution)
5. [verify that services are running](#key-http-routes)

## Shared docker network
```shell
cd docker
./create-shared-network.sh
```

## Observability

To start observability do:
```shell
cd docker/observability
docker compose up -d
```

Open Grafana at http://localhost:3000 and log in (default credentials are admin/admin).
For the first time grafana will ask you to set new admin password

### OpenTelemetry Tracing

- Metrics: Prometheus http://localhost:9090
- Tracing: Tempo http://localhost:3200
- Logging: Loki http://localhost:3100

### Grafana Dashboards 
The following dashboards are provisioned 
- [ASP.NET OTEL (OpenTelemetry) Metrics](https://grafana.com/grafana/dashboards/17706-asp-net-otel-metrics/?pg=hp)
- [Prometheus 2.0 Overview](https://grafana.com/grafana/dashboards/3662-prometheus-2-0-overview/)

## Running the solution

in docker folder run
```shell
cd docker
./build-docker.sh
docker compose up -d
```

### Petabridge.Cmd Support

This project is designed to work with [Petabridge.Cmd](https://cmd.petabridge.com/). For instance, if you want to check with the status of your Akka.NET Cluster, just run:

```shell
pbm cluster show
```

Once cluster is formed you should see something like this:

```shell
akka.tcp://aaron@523f3a1d0e24:8557 | [matchingengine] | up |  | 1.0.0
akka.tcp://aaron@6c7a408130b7:8557 | [matchingengine] | up |  | 1.0.0
akka.tcp://aaron@738938ed56b1:8557 | [matchingengine] | joining |  | 1.0.0
Count: 3 nodes
```
above shows that 1 node is joining the cluster, and after cluster is fully formed like this:
```shell
akka.tcp://aaron@523f3a1d0e24:8557 | [matchingengine] | up |  | 1.0.0
akka.tcp://aaron@6c7a408130b7:8557 | [matchingengine] | up |  | 1.0.0
akka.tcp://aaron@738938ed56b1:8557 | [matchingengine] | up |  | 1.0.0
Count: 3 nodes
```

### Key HTTP Routes

* https://localhost:{ASP_NET_PORT}/swagger/index.html - Swagger endpoint for testing out Akka.NET-powered APIs
* https://localhost:{ASP_NET_PORT}/healthz/akka - Akka.HealthCheck HTTP endpoint
* https://localhost:{ASP_NET_PORT}/_env - service information HTTP endpoint, more like a convient endpoint to quickly check the service info

ASP_NET_PORT if you run locally it will most likly be set to 5000, but when running in docker we expose ports from 8080 upto number of replicas -1

* https://localhost:8080/swagger/index.html - Swagger endpoint for testing out Akka.NET-powered APIs
* https://localhost:8080/healthz/akka - Akka.HealthCheck HTTP endpoint
* https://localhost:8080/_env - service information HTTP endpoint, more like a convient endpoint to quickly check the service info
