# Docker Rootless Mode Setup on Debian 13 (Trixie)

Running Docker in rootless mode eliminates the need for `sudo` and improves security by not requiring elevated privileges. This guide covers setting up rootless Docker on Debian 13 (Trixie).

## Prerequisites

Ensure your system has the required packages for rootless mode:

```bash
sudo apt-get update
sudo apt-get install -y uidmap dbus-user-session slirp4netns fuse-overlayfs
```

**What each package does:**
- `uidmap` - Required for user namespace mapping
- `dbus-user-session` - Enables per-user D-Bus session needed for rootless systemd integration
- `slirp4netns` - User-space network stack (default networking mode)
- `fuse-overlayfs` - Optional but recommended for better performance with overlay2 storage driver

## Install Docker with Rootless Support

### 1. Remove Any Existing Docker Installation

If you have rootful Docker installed and want to disable it:

```bash
sudo systemctl disable docker.service
sudo systemctl stop docker.service
```

### 2. Add Docker Official Repository

```bash
# Add Docker's official GPG key
curl -fsSL https://download.docker.com/linux/debian/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker-archive-keyring.gpg

# Add the Docker repository
echo \
  "deb [arch=amd64 signed-by=/etc/apt/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/debian \
  $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

sudo apt-get update
```

### 3. Install Docker with Rootless Extras

```bash
sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-compose-plugin docker-ce-rootless-extras
```

The `docker-ce-rootless-extras` package includes the `dockerd-rootless-setuptool.sh` script.

## Configure Rootless Mode

### 1. Run the Setup Script

As your regular (non-root) user:

```bash
dockerd-rootless-setuptool.sh install
```

This script will:
- Create necessary user namespaces and cgroups
- Set up the rootless daemon socket
- Configure systemd user service files
- Display the `DOCKER_HOST` environment variable to use

### 2. Export DOCKER_HOST

Add the following to your shell profile (`~/.bashrc`, `~/.zshrc`, or similar):

```bash
export DOCKER_HOST=unix:///run/user/$(id -u)/docker.sock
```

Then reload your shell:

```bash
source ~/.bashrc  # or ~/.zshrc
```

Verify it's set:

```bash
echo $DOCKER_HOST
# Output: unix:///run/user/1000/docker.sock (your UID will vary)
```

### 3. Enable and Start the Rootless Daemon

```bash
# Enable the service to start on login
systemctl --user enable docker

# Start the service
systemctl --user start docker

# Verify it's running
systemctl --user status docker
```

### 4. Persist Docker Between Sessions (Optional but Recommended)

By default, user services stop when you log out. To keep Docker running:

```bash
sudo loginctl enable-linger $(whoami)
```

Verify:

```bash
sudo loginctl show-user $(whoami) | grep Linger
# Output: Linger=yes
```

## Verify Installation

```bash
docker run hello-world
```

You should see "Hello from Docker!" without using `sudo`.

## Using Docker Compose with Rootless Mode

Docker Compose v2 (the `docker compose` command, included as a plugin in `docker-compose-plugin`) works automatically with rootless Docker via the `DOCKER_HOST` environment variable. No additional configuration needed:

```bash
docker compose up cosmosdb
```

If you're using the older `docker-compose` binary (v1), ensure `DOCKER_HOST` is exported in the same shell session where you run the command.

## Common Issues and Troubleshooting

### Issue: "docker: command not found"

**Solution:** Make sure the Docker socket path is exported in your current shell session:

```bash
export DOCKER_HOST=unix:///run/user/$(id -u)/docker.sock
docker ps
```

### Issue: Permission denied on Docker socket

**Solution:** Ensure the rootless daemon is running and the socket file exists:

```bash
systemctl --user is-active docker
# Check socket
ls -l /run/user/$(id -u)/docker.sock
```

### Issue: "bridge" network fails with IPv4 binding

**Known on Debian 13:** This is a cgroup v2 + slirp4netns interaction. Use `passt` (newer, better) instead:

```bash
sudo apt-get install passt
echo "network_backend = \"passt\"" >> ~/.config/docker/daemon.json
systemctl --user restart docker
```

Or disable IPv4 binding in containers requiring it.

### Issue: Storage driver is "vfs" (very slow)

**Solution:** Debian 13 defaults to cgroup v2, which supports `fuse-overlayfs`. Configure it:

```bash
mkdir -p ~/.config/docker
cat > ~/.config/docker/daemon.json << 'EOF'
{
  "storage-driver": "overlay2",
  "storage-opts": ["overlay2.override_kernel_check=true"]
}
EOF
systemctl --user restart docker
```

Verify:

```bash
docker info | grep "Storage Driver"
# Should show: overlay2
```

### Issue: Rootful Docker still exists and conflicts

If you want to keep both:
- Rootful runs on `/run/docker.sock` (requires sudo)
- Rootless runs on `/run/user/UID/docker.sock` (no sudo)

To avoid confusion, disable rootful:

```bash
sudo systemctl disable docker.service
sudo systemctl mask docker.service
```

## References

- [Docker Official Docs: Run the Docker daemon as a non-root user (rootless mode)](https://docs.docker.com/engine/security/rootless/)
- [Debian 13 Cgroup v2 Documentation](https://wiki.debian.org/cgroups)
