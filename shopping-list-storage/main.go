package main

import (
	"flag"
	"fmt"
	"log"
	"net"

	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/shoppinglist"
	"google.golang.org/grpc"
	"google.golang.org/grpc/reflection"
	"gopkg.in/DataDog/dd-trace-go.v1/ddtrace/tracer"
)

func main() {
	tracer.Start(
		tracer.WithEnv("local"),
		tracer.WithService("chat-message-storage"),
		tracer.WithServiceVersion("v1.1.1"),
	)
	defer tracer.Stop()
	flag.Parse()
	lis, err := net.Listen("tcp", fmt.Sprintf("localhost:%d", 9000))
	if err != nil {
		log.Fatalf("failed to listen: %v", err)
	}
	grpcServer := grpc.NewServer()
	reflection.Register(grpcServer)
	shoppinglist.RegisterShoppingListsStorageServer(grpcServer, NewFakeCustomerShoppingListServer())
	grpcServer.Serve(lis)
}
