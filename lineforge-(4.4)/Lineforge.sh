#!/bin/sh
echo -ne '\033c\033]0;Lineforge\a'
base_path="$(dirname "$(realpath "$0")")"
"$base_path/Lineforge.x86_64" "$@"
