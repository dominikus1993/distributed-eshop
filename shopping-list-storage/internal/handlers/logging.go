package handlers

import (
	"os"

	"github.com/sirupsen/logrus"
	log "github.com/sirupsen/logrus"
	"github.com/uptrace/opentelemetry-go-extra/otellogrus"
)

func initLogger() *logrus.Logger {
	logger := log.New()
	// Log as JSON instead of the default ASCII formatter.
	logger.SetFormatter(&log.JSONFormatter{})

	// Output to stdout instead of the default stderr
	// Can be any io.Writer, see below for File example
	logger.SetOutput(os.Stdout)

	// Only log the warning severity or above.
	logger.SetLevel(log.InfoLevel)
	logger.AddHook(otellogrus.NewHook(otellogrus.WithLevels(
		logrus.PanicLevel,
		logrus.FatalLevel,
		logrus.ErrorLevel,
		logrus.WarnLevel,
	)))
	return logger
}
