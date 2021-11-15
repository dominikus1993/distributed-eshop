package cmd

import (
	"context"
	"flag"
	"fmt"
	"log"
	"net"

	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/handlers"
	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/shoppinglist"
	"github.com/google/subcommands"
	"google.golang.org/grpc"
	"google.golang.org/grpc/reflection"
	grpctrace "gopkg.in/DataDog/dd-trace-go.v1/contrib/google.golang.org/grpc"
	"gopkg.in/DataDog/dd-trace-go.v1/ddtrace/tracer"
)

type RunGrpcServer struct {
}

func (*RunGrpcServer) Name() string     { return "run-grpc" }
func (*RunGrpcServer) Synopsis() string { return "run grpc server" }
func (*RunGrpcServer) Usage() string {
	return `go run . run-grpc`
}

func (p *RunGrpcServer) SetFlags(f *flag.FlagSet) {
}

func (p *RunGrpcServer) Execute(ctx context.Context, f *flag.FlagSet, _ ...interface{}) subcommands.ExitStatus {
	tracer.Start(
		tracer.WithEnv("local"),
		tracer.WithService("shopping-list-storage"),
		tracer.WithServiceVersion("v1.1.1"),
	)
	defer tracer.Stop()
	flag.Parse()
	lis, err := net.Listen("tcp", fmt.Sprintf(":%d", 9000))
	if err != nil {
		log.Fatalf("failed to listen: %w", err)
		return subcommands.ExitFailure
	}
	server, err := handlers.InitRpc()
	if err != nil {
		log.Fatalln(err)
	}
	defer server.Close(ctx)
	si := grpctrace.StreamServerInterceptor(grpctrace.WithServiceName("shopping-list-storage"))
	ui := grpctrace.UnaryServerInterceptor(grpctrace.WithServiceName("shopping-list-storage"))
	grpcServer := grpc.NewServer(grpc.StreamInterceptor(si), grpc.UnaryInterceptor(ui))
	reflection.Register(grpcServer)
	shoppinglist.RegisterShoppingListsStorageServer(grpcServer, server)
	if err := grpcServer.Serve(lis); err != nil {
		log.Fatalf("failed to serve: %w", err)
		return subcommands.ExitFailure
	}
	return subcommands.ExitSuccess
}
