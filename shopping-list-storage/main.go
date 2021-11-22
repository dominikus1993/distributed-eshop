package main

import (
	"context"
	"flag"
	"os"

	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/cmd"
	"github.com/google/subcommands"
)

func main() {
	subcommands.Register(&cmd.RunGinApiServer{}, "")
	subcommands.Register(&cmd.RunGrpcServer{}, "")
	flag.Parse()
	ctx := context.Background()
	os.Exit(int(subcommands.Execute(ctx)))
}
