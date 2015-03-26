"use strict";

/*
 * This module can be used to capture stdout and stderr from an Edge process.
 * Calling accumulateMessages() causes messages to be accumulated in a variable;
 * when the process is done you can retrieve the messages via the getMessages()
 * function.
 */

var msgs = [];

module.exports = {
    accumulateMessages : function() {
        process.stdout.write = function (string) {
            msgs.push({ stream: 'o', message : string });
        };
        process.stderr.write = function (string) {
            msgs.push({ stream: 'e', message: string });
        };
    },

    getMessages: function () {
        return msgs;
    }
};
