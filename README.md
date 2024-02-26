# QuartzClusteringPoC
[![Build](https://github.com/ArturMarekNowak/QuartzClusteringPoC/actions/workflows/workflow.yml/badge.svg)](https://github.com/ArturMarekNowak/QuartzClusteringPoC/actions/workflows/workflow.yml/badge.svg) [![Trivy and dockler](https://github.com/ArturMarekNowak/QuartzClusteringPoC/actions/workflows/image-scan.yml/badge.svg)](https://github.com/ArturMarekNowak/QuartzClusteringPoC/actions/workflows/image-scan.yml/badge.svg) [![CodeFactor](https://www.codefactor.io/repository/github/arturmareknowak/QuartzClusteringPoC/badge)](https://www.codefactor.io/repository/github/arturmareknowak/QuartzClusteringPoC)

This project implements jobs executions in one replica only in multi-replica application  with utilization of Quart.NET library

## Table of contents
* [General info](#general-info)
* [Technologies](#technologies)
* [Setup](#setup)
* [Status](#status)
* [Inspiration](#inspiration)

## General info

The implementation is really simple as Quartz.NET comes with out-of-the-box clustering mechanism. Database contains a proper schema which provides reliable and only source of truth about executed jobs. Replicas communicate with the database and aware who is executing job, if execution is running now etc. Failovers and retries is also available.

<p align="center"><img src="./docs/FailedInstance.png"/>
<p align="center">Pic.1 Example of failover mechanism</p>

Overall project consists of four docker containers. Three with .NET API and one with postgres database.

<p align="center"><img src="./docs/network.png">
<p align="center">Pic.2 Visualization of docker compose project</p>


## Technologies
* .NET 8
* Postgres
* Docker
* Quartz.NET

## Setup
1. Run docker compose in src folder: `docker-compose up`

## Status
Project is: _finished_

## Inspiration
Shower thoughts