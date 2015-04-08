"use strict";

/*
 * This module can be used to capture stdout and stderr from an Edge process.
 * Calling accumulateMessages() causes messages to be accumulated in a variable;
 * when the process is done you can retrieve the messages via the getMessages()
 * function.
 *
 * Alternatively, calling hookStreams() with your Edge payload will result
 * in the hook functions you pass being called whenever a process writes to
 * stdout or stderr.
 */

var msgs = [];

exports.accumulateMessages = function() {
    process.stdout.write = function(string) {
        msgs.push({ stream: 'o', message : string });
    };
    process.stderr.write = function(string) {
        msgs.push({ stream: 'e', message: string });
    };
};

exports.getMessages = function () {
    return msgs;
};

exports.hookStreams = function(payload) {
    if (typeof (payload.stdoutHook) === 'function') {
        process.stdout.write = payload.stdoutHook;
    } else {
        //console.log('No stdoutHook is defined, typeof returns ' + typeof(payload.stdoutHook));
    }

    if (typeof (payload.stderrHook) === 'function') {
        process.stderr.write = payload.stderrHook;
    } else {
        //console.log('No stderrHook is defined, typeof returns ' + typeof(payload.stdoutHook));
    }
};
