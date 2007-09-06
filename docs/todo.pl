#!/usr/bin/perl

use strict;
use warnings;

sub push_task
{
  my ($tasks, $name, $value) = @_;
  if (@$tasks && !exists $tasks->[-1]->{$name}) {
    $tasks->[-1]->{$name} = $value;
  } else {
    push @$tasks, { $name => $value };
  }
}

my @tasks = ();
while (<>) {
  chomp;
  s/\s+/ /;
  s/^ //;
  s/ $//;
  s/ ?#.*$//;
  next if !/\S/;
  if (s/^what\? ?//i) { push_task(\@tasks, what => $') }
  elsif (s/^who\? ?//i) { push_task(\@tasks, who => $') }
  elsif (s/^when\? ?//i) { push_task(\@tasks, when => $') }
  elsif (s/^done\? ?//i) { push_task(\@tasks, done => $') }
  elsif (s/^and\? ?//i) { push_task(\@tasks, "and" => $') }
  else { print STDERR "Skipped \"$_\"\n"; }
}

sub html_task
{
  my ($task) = @_;
  return if !exists $task->{what};
  my $when = exists $task->{when} ? $task->{when} : "???";
  my $who = exists $task->{who} ? $task->{who} : "???";
  my $rowspan = exists $task->{"and"} ? qq( rowspan="2") : "";
  my $done = exists $task->{done} ? $task->{done} : "NO";
  $done = qq(<td valign="top"$rowspan class="$done">$done</td>);
  print <<HTML;
        <tr>
          $done
          <td valign="top" class="what">$task->{what}</td>
          <td valign="top">$when</td>
          <td valign="top">$who</td>
        </tr>
HTML
  if (exists $task->{"and"}) {
    print <<HTML;
        <tr>
          <td colspan="3">$task->{and}</td>
        </tr>
HTML
  }
}

my $title = "Task list for Obi";
my $time = localtime();
print <<HTML;
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN"
  "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
  <head>
    <title>$title</title>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/>
    <style type="text/css">
      table { border-collapse: collapse; }
      td { border: solid thin black; padding: .5em; }
      .NO { background-color: red; }
      .NOTYET { background-color: orange; }
      .YES { background-color: green; }
      .LATER { background-color: pink; }
      .what { font-weight: bold; font-size: 120% }

      body { background-color: white; color: black; margin: 3em; text-align:
        left; font-family: serif; }
      h1, h2, h3 { font-family: "Gill Sans", sans-serif; color: #04a }
      h1 { font-size: 150%; letter-spacing: .2em; text-align: center }
      h2 { font-size: 120%; }
      h3 { font-size: 100%; color: black }
      .in { text-indent: 1em }
      code { font-size: 90% }
      a { color: #009; font-weight: bold; text-decoration: none }
    </style>
  </head>
  <body>
    <h1>$title</h1>
    <p>Generated on: $time</p>
    <ul>
      <li><a href="#all">All tasks</a></li>
      <li><a href="#who">Tasks by person</a>
      <ul>
        <li><a href="#avneesh">Avneesh</a></li>
        <li><a href="#dipendra">Dipendra</a></li>
        <li><a href="#julien">Julien</a></li>
        <li><a href="#marisa">Marisa</a></li>
      </ul>
      </li>
      <li><a href="#status">Tasks by status</a>
      <ul>
        <li><a href="#NO" class="NO">NO</a> (not done)</li>
        <li><a href="#NOTYET" class="NOTYET">NOTYET</a> (in progress)</li>
        <li><a href="#YES" class="YES">YES</a> (done)</li>
        <li><a href="#LATER" class="LATER">LATER</a> (postponed)</li>
      </ul>
      </li>
    </ul>
    <h2 id="all">All tasks</h2>
    <table>
      <tbody>
HTML

html_task($_) for @tasks;

print <<HTML;
      </tbody>
    </table>
    <h2 id="who">Tasks by person</h2>
HTML

for my $who (qw(Avneesh Dipendra Julien Marisa)) {
  my $id = lc($who);
  print <<HTML;
    <h3 id="$id">$who</h3>
    <table>
      <tbody>
HTML

  html_task($_) for grep { $_->{who} =~ /\b$who\b/ || $_->{who} =~ /\ball\b/i }
    @tasks;

  print <<HTML;
      </tbody>
    </table>
HTML
}

print <<HTML;
    <h2 id="status">Tasks by status</h2>
HTML

for my $done (qw(NO NOTYET YES LATER)) {
  print <<HTML;
    <h3 id="$done">$done</h3>
    <table>
      <tbody>
HTML

  html_task($_) for grep { (exists $_->{done} && $_->{done} eq $done) ||
                           (!exists $_->{done} && $done eq "NO") } @tasks;

  print <<HTML;
      </tbody>
    </table>
HTML
}


print <<HTML;
  </body>
</html>
HTML
