package main

import (
	"flag"
	"fmt"
	"log"
	"net"

	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/shoppinglist"
	"google.golang.org/grpc"
	"google.golang.org/grpc/reflection"
	grpctrace "gopkg.in/DataDog/dd-trace-go.v1/contrib/google.golang.org/grpc"
	"gopkg.in/DataDog/dd-trace-go.v1/ddtrace/tracer"
)

func main() {
	tracer.Start(
		tracer.WithEnv("local"),
		tracer.WithService("shopping-list-storage"),
		tracer.WithServiceVersion("v1.1.1"),
	)
	defer tracer.Stop()
	flag.Parse()
	lis, err := net.Listen("tcp", fmt.Sprintf(":%d", 9000))
	if err != nil {
		log.Fatalf("failed to listen: %v", err)
	}
	si := grpctrace.StreamServerInterceptor(grpctrace.WithServiceName("shopping-list-storage"))
	ui := grpctrace.UnaryServerInterceptor(grpctrace.WithServiceName("shopping-list-storage"))
	grpcServer := grpc.NewServer(grpc.StreamInterceptor(si), grpc.UnaryInterceptor(ui))
	reflection.Register(grpcServer)
	shoppinglist.RegisterShoppingListsStorageServer(grpcServer, NewFakeCustomerShoppingListServer())

	if err := grpcServer.Serve(lis); err != nil {
		log.Fatalf("failed to serve: %v", err)
	}
}
