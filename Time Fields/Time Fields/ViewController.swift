//
//  ViewController.swift
//  Time Fields
//
//  Created by Andrew Downing on 6/9/16.
//  Copyright © 2016 Andrew Downing. All rights reserved.
//
//  Based on https://www.hackingwithswift.com/read/4/2/creating-a-simple-browser-with-wkwebview

import UIKit
import WebKit

class ViewController: UIViewController, WKNavigationDelegate {

    var webView: WKWebView!
    
    override func loadView() {
        webView = WKWebView()
        webView.navigationDelegate = self
        view = webView
    }

    override func viewDidLoad() {
        super.viewDidLoad()
        webView.loadRequest(NSURLRequest(URL: NSURL(string: "https://ad510.github.io/time-dilation-game")!))
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }
}

