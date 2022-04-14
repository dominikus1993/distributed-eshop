package cmd

import (
	"context"
	"flag"

	log "github.com/sirupsen/logrus"

	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/internal/handlers"
	"github.com/google/subcommands"
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
	log.Println("Start")
	api, err := handlers.InitApi(ctx)
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
