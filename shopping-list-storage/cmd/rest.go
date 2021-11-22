package cmd

import (
	"context"
	"flag"
	"log"

	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/handlers"
	"github.com/google/subcommands"
	"gopkg.in/DataDog/dd-trace-go.v1/ddtrace/tracer"
)

type RunGinApiServer struct {
}

func (*RunGinApiServer) Name() string     { return "run-gin-api" }
func (*RunGinApiServer) Synopsis() string { return "run gin api" }
func (*RunGinApiServer) Usage() string {
	return `go run . run-gin-api`
}

func (p *RunGinApiServer) SetFlags(f *flag.FlagSet) {
}

func (p *RunGinApiServer) Execute(ctx context.Context, f *flag.FlagSet, _ ...interface{}) subcommands.ExitStatus {
	tracer.Start(
		tracer.WithEnv("local"),
		tracer.WithService("shopping-list-storage"),
		tracer.WithServiceVersion("v1.1.1"),
	)
	defer tracer.Stop()
	log.Println("Start")
	api, err := handlers.InitApi()
	if err != nil {
		log.Fatalln(err)
		return subcommands.ExitFailure
	}

	defer api.Close(ctx)

	if err := api.Start(ctx); err != nil {
		log.Fatal(err)
		return subcommands.ExitFailure
	}
	return subcommands.ExitSuccess
}
