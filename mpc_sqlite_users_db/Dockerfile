# Use a lightweight base image with Alpine Linux
FROM alpine:latest

# Install SQLite
RUN apk --no-cache add sqlite sqlite-dev

# Create a directory for the SQLite database
RUN mkdir -p /data

# Set the working directory
WORKDIR /data

# Optionally copy an existing SQLite database into the container
COPY Users.db /data/Users.db

# Expose the port your application will run on
EXPOSE 80

# Set the default command to run SQLite
CMD ["sqlite3"]
