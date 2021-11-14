package main

import (
	"context"
	"log"

	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/handlers"
	"gopkg.in/DataDog/dd-trace-go.v1/ddtrace/tracer"
)

func main() {
	ctx := context.TODO()
	tracer.Start(
		tracer.WithEnv("local"),
		tracer.WithService("shopping-list-storage"),
		tracer.WithServiceVersion("v1.1.1"),
	)
	defer tracer.Stop()
	api, err := handlers.Init()
	if err != nil {
		log.Fatalln(err)
	}

	defer api.Close(ctx)

	if err := api.Start(ctx); err != nil {
		panic(err)
	}
}
