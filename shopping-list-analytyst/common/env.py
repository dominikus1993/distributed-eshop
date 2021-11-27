import os


def get_env_or_default(var_name: str, default: str) -> str:
    return os.environ[var_name] if var_name in os.environ else default